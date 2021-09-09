using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Model.Configurations
{
	// Token: 0x02000064 RID: 100
	public class FieldsToPropertiesTypeDescriptionProvider : TypeDescriptionProvider
	{
		// Token: 0x0600022E RID: 558 RVA: 0x0000594F File Offset: 0x00003B4F
		public FieldsToPropertiesTypeDescriptionProvider(Type t)
		{
			this._baseProvider = TypeDescriptor.GetProvider(t);
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00005963 File Offset: 0x00003B63
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			return new FieldsToPropertiesTypeDescriptionProvider.FieldsToPropertiesTypeDescriptor(this, this._baseProvider.GetTypeDescriptor(objectType, instance), objectType);
		}

		// Token: 0x040001EA RID: 490
		private readonly TypeDescriptionProvider _baseProvider;

		// Token: 0x040001EB RID: 491
		private PropertyDescriptorCollection _propCache;

		// Token: 0x040001EC RID: 492
		private FieldsToPropertiesTypeDescriptionProvider.FilterCache _filterCache;

		// Token: 0x02000065 RID: 101
		private class FilterCache
		{
			// Token: 0x06000230 RID: 560 RVA: 0x0000598C File Offset: 0x00003B8C
			public bool IsValid(Attribute[] other)
			{
				return other != null && this.Attributes != null && this.Attributes.Length == other.Length && !other.Where((Attribute t, int i) => !this.Attributes[i].Match(t)).Any<Attribute>();
			}

			// Token: 0x040001ED RID: 493
			public Attribute[] Attributes;

			// Token: 0x040001EE RID: 494
			public PropertyDescriptorCollection FilteredProperties;
		}

		// Token: 0x02000066 RID: 102
		public class FieldPropertyDescriptor : PropertyDescriptor
		{
			// Token: 0x06000233 RID: 563 RVA: 0x000059CC File Offset: 0x00003BCC
			public FieldPropertyDescriptor(FieldInfo field) : base(field.Name, (Attribute[])field.GetCustomAttributes(typeof(Attribute), true))
			{
				this._field = field;
			}

			// Token: 0x170000B3 RID: 179
			// (get) Token: 0x06000234 RID: 564 RVA: 0x000059F7 File Offset: 0x00003BF7
			public FieldInfo Field
			{
				get
				{
					return this._field;
				}
			}

			// Token: 0x06000235 RID: 565 RVA: 0x00005A00 File Offset: 0x00003C00
			public override bool Equals(object obj)
			{
				FieldsToPropertiesTypeDescriptionProvider.FieldPropertyDescriptor fieldPropertyDescriptor = obj as FieldsToPropertiesTypeDescriptionProvider.FieldPropertyDescriptor;
				return fieldPropertyDescriptor != null && fieldPropertyDescriptor._field.Equals(this._field);
			}

			// Token: 0x06000236 RID: 566 RVA: 0x00005A2A File Offset: 0x00003C2A
			public override int GetHashCode()
			{
				return this._field.GetHashCode();
			}

			// Token: 0x170000B4 RID: 180
			// (get) Token: 0x06000237 RID: 567 RVA: 0x00005A37 File Offset: 0x00003C37
			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			// Token: 0x06000238 RID: 568 RVA: 0x00005A3A File Offset: 0x00003C3A
			public override void ResetValue(object component)
			{
			}

			// Token: 0x06000239 RID: 569 RVA: 0x00005A3C File Offset: 0x00003C3C
			public override bool CanResetValue(object component)
			{
				return false;
			}

			// Token: 0x0600023A RID: 570 RVA: 0x00005A3F File Offset: 0x00003C3F
			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}

			// Token: 0x170000B5 RID: 181
			// (get) Token: 0x0600023B RID: 571 RVA: 0x00005A42 File Offset: 0x00003C42
			public override Type ComponentType
			{
				get
				{
					return this._field.DeclaringType;
				}
			}

			// Token: 0x170000B6 RID: 182
			// (get) Token: 0x0600023C RID: 572 RVA: 0x00005A4F File Offset: 0x00003C4F
			public override Type PropertyType
			{
				get
				{
					return this._field.FieldType;
				}
			}

			// Token: 0x0600023D RID: 573 RVA: 0x00005A5C File Offset: 0x00003C5C
			public override object GetValue(object component)
			{
				return this._field.GetValue(component);
			}

			// Token: 0x0600023E RID: 574 RVA: 0x00005A6A File Offset: 0x00003C6A
			public override void SetValue(object component, object value)
			{
				this._field.SetValue(component, value);
				this.OnValueChanged(component, EventArgs.Empty);
			}

			// Token: 0x040001EF RID: 495
			private readonly FieldInfo _field;
		}

		// Token: 0x02000067 RID: 103
		private class FieldsToPropertiesTypeDescriptor : CustomTypeDescriptor
		{
			// Token: 0x0600023F RID: 575 RVA: 0x00005A88 File Offset: 0x00003C88
			public FieldsToPropertiesTypeDescriptor(FieldsToPropertiesTypeDescriptionProvider provider, ICustomTypeDescriptor descriptor, Type objectType) : base(descriptor)
			{
				if (provider == null)
				{
					throw new ArgumentNullException("provider");
				}
				if (descriptor == null)
				{
					throw new ArgumentNullException("descriptor");
				}
				if (objectType == null)
				{
					throw new ArgumentNullException("objectType");
				}
				this._objectType = objectType;
				this._provider = provider;
			}

			// Token: 0x06000240 RID: 576 RVA: 0x00005ADA File Offset: 0x00003CDA
			public override PropertyDescriptorCollection GetProperties()
			{
				return this.GetProperties(null);
			}

			// Token: 0x06000241 RID: 577 RVA: 0x00005AE4 File Offset: 0x00003CE4
			public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
			{
				bool flag = attributes != null && attributes.Length > 0;
				FieldsToPropertiesTypeDescriptionProvider.FilterCache filterCache = this._provider._filterCache;
				PropertyDescriptorCollection propertyDescriptorCollection = this._provider._propCache;
				if (flag && filterCache != null && filterCache.IsValid(attributes))
				{
					return filterCache.FilteredProperties;
				}
				if (!flag && propertyDescriptorCollection != null)
				{
					return propertyDescriptorCollection;
				}
				propertyDescriptorCollection = new PropertyDescriptorCollection(null);
				foreach (object obj in base.GetProperties(attributes))
				{
					PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj;
					if (propertyDescriptor != null)
					{
						propertyDescriptor.GetValue(this);
						propertyDescriptor.SetValue(this, 22);
						propertyDescriptorCollection.Add(propertyDescriptor);
					}
				}
				foreach (FieldInfo field in this._objectType.GetFields())
				{
					FieldsToPropertiesTypeDescriptionProvider.FieldPropertyDescriptor fieldPropertyDescriptor = new FieldsToPropertiesTypeDescriptionProvider.FieldPropertyDescriptor(field);
					if (!flag || fieldPropertyDescriptor.Attributes.Contains(attributes))
					{
						propertyDescriptorCollection.Add(fieldPropertyDescriptor);
					}
				}
				if (flag)
				{
					filterCache = new FieldsToPropertiesTypeDescriptionProvider.FilterCache
					{
						FilteredProperties = propertyDescriptorCollection,
						Attributes = attributes
					};
					this._provider._filterCache = filterCache;
				}
				else
				{
					this._provider._propCache = propertyDescriptorCollection;
				}
				return propertyDescriptorCollection;
			}

			// Token: 0x040001F0 RID: 496
			private readonly Type _objectType;

			// Token: 0x040001F1 RID: 497
			private readonly FieldsToPropertiesTypeDescriptionProvider _provider;
		}
	}
}
