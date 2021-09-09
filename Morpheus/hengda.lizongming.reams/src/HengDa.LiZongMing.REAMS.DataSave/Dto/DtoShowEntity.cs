using Aming.Core;
using Castle.Core.Logging;
using PropertyChanged;
//using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS.Wpf.Dto
{
    [AddINotifyPropertyChangedInterface]
    public class DtoShowEntity
    {
        public DtoShowEntity(int iDefaultRow=20)
        {
            //TODO:JPS,这里直接初始化并添加20行数据，因为还未解决MVVM模式下数据源新增行后，DataGrid没有显示新增的行的问题
            this.Datas = new List<MyViewEntity>();
            for (int i = 0; i < iDefaultRow; i++)
            {
                this.Datas.Add(new MyViewEntity()
                {
                    Title = "",
                    Value = ""
                });
            }
        }
        
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
        public List<MyViewEntity> Datas { get; set; }
        public void AddItem(string sTitle, string sValue)
        {
            if (this.Datas == null)
                this.Datas = new List<MyViewEntity>();
            this.Datas.Add(new MyViewEntity()
            {
                Title = sTitle,
                Value = sValue
            });
        }
        public void DataClear()
        {
            if (this.Datas != null)
                this.Datas.Clear();
        }
        public void SetValue(string sTitle, string sValue, string sUnitName)
        {
            if (this.Datas == null) this.Datas = new List<MyViewEntity>();
            MyViewEntity data = this.Datas.Find(delegate (MyViewEntity temp)
            {
                return string.Compare(sTitle, temp.Title, true) == 0;
            });
            if (data == null)
            {
                this.Datas.Add(new MyViewEntity()
                {
                    Title = sTitle,
                    Value = sValue,
                    UnitName = sUnitName
                });
            }
            else
            {
                if (data.Value != sValue)
                    data.Value = sValue;
            }
        }
        /// <summary>
        /// 按顺序添加信息，暂时用这个函数添加信息
        /// </summary>
        /// <param name="objDto">需要显示的DTO数据实例</param>
        public void SetDotValuesOrder(object objDto)
        {
            System.Reflection.PropertyInfo[] propertys = objDto.GetType().GetProperties();
            object[] atrs;
            System.ComponentModel.DescriptionAttribute desc;
            string strDesc;
            object objValue;
            int iDataIndex = 0;
            string strTitle, strUnit;
            for (int i = 0; i < propertys.Length; i++)
            {
                if (iDataIndex >= this.Datas.Count)
                {
                    //此时说明初始化该对象时定义明细行数太少了。
                    break;
                }
                System.Reflection.PropertyInfo pinfo = propertys[i];
                objValue = pinfo.GetValue(objDto, null);
                if (objValue == null)
                    objValue = string.Empty;
                if (string.Compare("ErrMsg", pinfo.Name, true) == 0)
                {
                    //消息不用列表显示，字节用大文本框显示出来
                    this.Msg = objValue.ToString();
                    continue;
                }
                atrs = pinfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                if (atrs.Length > 0)
                {
                    desc = (System.ComponentModel.DescriptionAttribute)atrs[0];
                    if (desc == null) strDesc = pinfo.Name;
                    else strDesc = desc.Description;
                }
                else
                {
                    strDesc = pinfo.Name;
                }
                GetTitlAndUnit(strDesc, out strTitle, out strUnit);
                if (this.Datas[iDataIndex].Value != objValue.ToString())
                    this.Datas[iDataIndex].Value = objValue.ToString();
                if (this.Datas[iDataIndex].Title != strTitle)
                    this.Datas[iDataIndex].Title = strTitle;
                if (this.Datas[iDataIndex].UnitName != strUnit)
                    this.Datas[iDataIndex].UnitName = strUnit;
                iDataIndex++;
            }

            for (; iDataIndex < this.Datas.Count; iDataIndex++)
            {
                this.Datas[iDataIndex].Title = "";
                this.Datas[iDataIndex].Value = "";
                this.Datas[iDataIndex].UnitName = "";
            }
        }
        /// <summary>
        /// 根据传入对象添加显示内容，暂时这个函数不要用个，还未解决MVVM模式下数据源新增行后，DataGrid没有显示新增的行的问题
        /// </summary>
        /// <param name="objDto">需要显示的DTO数据实例</param>
        public void SetDotValues(object objDto)
        {
            System.Reflection.PropertyInfo[] propertys = objDto.GetType().GetProperties();
            object[] atrs;
            System.ComponentModel.DescriptionAttribute desc;
            string strDesc;
            object objValue;
            string strTitle, strUnit;
            foreach (System.Reflection.PropertyInfo pinfo in propertys)
            {
                objValue = pinfo.GetValue(objDto, null);
                if (objValue == null)
                    objValue = string.Empty;
                atrs = pinfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                if (atrs.Length > 0)
                {
                    desc = (System.ComponentModel.DescriptionAttribute)atrs[0];
                    if (desc == null) strDesc = pinfo.Name;
                    else strDesc = desc.Description;
                }
                else
                {
                    strDesc = pinfo.Name;
                }
                GetTitlAndUnit(strDesc, out strTitle, out strUnit);
                this.SetValue(strDesc, objValue.ToString(), strUnit);
                //Response.Write("<br>" + pinfo.Name + "," + pinfo.GetValue(myobj, null));
            }
        }
        private void GetTitlAndUnit(string sTitle, out string sTitleNew, out string sUnitNew)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"\[.*?\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match mc = reg.Match(sTitle);
            if (mc != null && mc.Value.Length > 0)
            {
                sUnitNew = mc.Value.Substring(1, mc.Value.Length - 2);
                sTitleNew = sTitle.Replace(mc.Value, "");
            }
            else
            {
                sUnitNew = string.Empty;
                sTitleNew = sTitle;
            }
        }
        public void SetMsg(string sMsg)
        {
            this.Msg = sMsg;
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class MyViewEntity
    {
        /// <summary>
        /// 参数标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }
    }
    [AddINotifyPropertyChangedInterface]
    public class DataGridResult<T> where T:new()
    {
        public List<T> DataSource { get; set; }
        public DataGridResult(int iDefaultCnt=20)
        {
            this.DataSource = new List<T>();
            for(int i=0;i<iDefaultCnt;i++)
            {
                this.DataSource.Add(new T());
            }
        }
        public T this[int index]
        {
            get { return DataSource[index]; }
            set { DataSource[index] = value; }
        }
        public int DataCount { get; set; } = 0;
        public void AddData(T data)
        {
            if (DataCount >= this.DataSource.Count) return;
            this.DataSource[DataCount] = data;
            this.DataCount++;
        }
        public void Clear()
        {
            for(int i=0;i<this.DataCount;i++)
            {
                this.DataSource[i] = new T();
            }
            this.DataCount = 0;
        }
    }
}
