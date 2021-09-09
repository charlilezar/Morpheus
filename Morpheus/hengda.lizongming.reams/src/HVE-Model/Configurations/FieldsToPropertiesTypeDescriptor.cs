using System;
using System.ComponentModel;
using System.Reflection;

namespace Model.Configurations
{
	// Token: 0x0200006C RID: 108
	public abstract class FieldsToPropertiesTypeDescriptor : ICustomTypeDescriptor
	{
		// Token: 0x0600028D RID: 653 RVA: 0x0000648D File Offset: 0x0000468D
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00006490 File Offset: 0x00004690
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00006499 File Offset: 0x00004699
		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x000064A2 File Offset: 0x000046A2
		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x000064AB File Offset: 0x000046AB
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x000064B4 File Offset: 0x000046B4
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		// Token: 0x06000293 RID: 659 RVA: 0x000064BD File Offset: 0x000046BD
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		// Token: 0x06000294 RID: 660 RVA: 0x000064C6 File Offset: 0x000046C6
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		// Token: 0x06000295 RID: 661 RVA: 0x000064D0 File Offset: 0x000046D0
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		// Token: 0x06000296 RID: 662 RVA: 0x000064DA File Offset: 0x000046DA
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		// Token: 0x06000297 RID: 663 RVA: 0x000064E3 File Offset: 0x000046E3
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		// Token: 0x06000298 RID: 664 RVA: 0x000064EC File Offset: 0x000046EC
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			bool flag = attributes != null && attributes.Length > 0;
			PropertyDescriptorCollection propertyDescriptorCollection = this._propCache;
			FieldsToPropertiesTypeDescriptor.FilterCache filterCache = this._filterCache;
			if (flag && filterCache != null && filterCache.IsValid(attributes))
			{
				return filterCache.FilteredProperties;
			}
			if (!flag && propertyDescriptorCollection != null)
			{
				return propertyDescriptorCollection;
			}
			IsFactoryAttribute attribute = new IsFactoryAttribute();
			propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			foreach (object obj in TypeDescriptor.GetProperties(this, attributes, true))
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj;
				if (!propertyDescriptor.Attributes.Contains(attribute) || (propertyDescriptor.Attributes.Contains(attribute) && RSDetectionConfiguration.DisplayFactoryConfiguration))
				{
					propertyDescriptorCollection.Add(propertyDescriptor);
				}
			}
			foreach (FieldInfo field in base.GetType().GetFields())
			{
				FieldPropertyDescriptor fieldPropertyDescriptor = new FieldPropertyDescriptor(field);
				if (!flag || fieldPropertyDescriptor.Attributes.Contains(attributes))
				{
					propertyDescriptorCollection.Add(fieldPropertyDescriptor);
				}
			}
			if (flag)
			{
				this._filterCache = new FieldsToPropertiesTypeDescriptor.FilterCache
				{
					Attributes = attributes,
					FilteredProperties = propertyDescriptorCollection
				};
			}
			else
			{
				this._propCache = propertyDescriptorCollection;
			}
			return propertyDescriptorCollection;
		}

		// Token: 0x04000213 RID: 531
		private PropertyDescriptorCollection _propCache;

		// Token: 0x04000214 RID: 532
		private FieldsToPropertiesTypeDescriptor.FilterCache _filterCache;

		// Token: 0x0200006D RID: 109
		private class FilterCache
		{
			// Token: 0x0600029A RID: 666 RVA: 0x00006638 File Offset: 0x00004838
			public bool IsValid(Attribute[] other)
			{
				if (other == null || this.Attributes == null)
				{
					return false;
				}
				if (this.Attributes.Length != other.Length)
				{
					return false;
				}
				for (int i = 0; i < other.Length; i++)
				{
					if (!this.Attributes[i].Match(other[i]))
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x04000215 RID: 533
			public Attribute[] Attributes;

			// Token: 0x04000216 RID: 534
			public PropertyDescriptorCollection FilteredProperties;
		}
	}
}
