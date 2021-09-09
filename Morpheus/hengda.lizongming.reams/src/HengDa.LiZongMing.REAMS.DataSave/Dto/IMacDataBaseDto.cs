namespace HengDa.LiZongMing.REAMS.Wpf.Dto
{
    /// <summary>
    /// 硬件通讯数据包解析出数据内容的基类
    /// </summary>
    public interface IMacDataBaseDto
    {
        /// <summary>
        /// hex原始数据
        /// </summary>
        byte[] Raw { get; set; }
    }
}