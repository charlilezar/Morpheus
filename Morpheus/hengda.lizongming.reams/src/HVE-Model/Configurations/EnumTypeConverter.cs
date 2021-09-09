using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Model.Configurations
{
	// Token: 0x02000062 RID: 98
	public class EnumTypeConverter : EnumConverter
	{
		// Token: 0x0600021D RID: 541 RVA: 0x000056DA File Offset: 0x000038DA
		public EnumTypeConverter(Type type) : base(type)
		{
			this._enumType = type;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x000056EA File Offset: 0x000038EA
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
		{
			return destType == typeof(string);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x000056FC File Offset: 0x000038FC
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
		{
			if (value == null)
			{
				return null;
			}
			FieldInfo field = this._enumType.GetField(Enum.GetName(this._enumType, value));
			DescriptionAttribute descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
			if (descriptionAttribute != null)
			{
				return descriptionAttribute.Description;
			}
			return value.ToString();
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000574C File Offset: 0x0000394C
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
		{
			return srcType == typeof(string);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00005760 File Offset: 0x00003960
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			foreach (FieldInfo fieldInfo in this._enumType.GetFields())
			{
				DescriptionAttribute descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
				if (descriptionAttribute != null && (string)value == descriptionAttribute.Description)
				{
					return Enum.Parse(this._enumType, fieldInfo.Name);
				}
			}
			return Enum.Parse(this._enumType, (string)value);
		}

		// Token: 0x040001E5 RID: 485
		private readonly Type _enumType;
	}
}
