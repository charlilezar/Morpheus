using System;
using System.ComponentModel;
using System.Reflection;

namespace Model.Configurations
{
	// Token: 0x0200006B RID: 107
	public class FieldPropertyDescriptor : PropertyDescriptor
	{
		// Token: 0x06000281 RID: 641 RVA: 0x000063D3 File Offset: 0x000045D3
		public FieldPropertyDescriptor(FieldInfo field) : base(field.Name, (Attribute[])field.GetCustomAttributes(typeof(Attribute), true))
		{
			this._field = field;
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000282 RID: 642 RVA: 0x000063FE File Offset: 0x000045FE
		public FieldInfo Field
		{
			get
			{
				return this._field;
			}
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00006408 File Offset: 0x00004608
		public override bool Equals(object obj)
		{
			FieldPropertyDescriptor fieldPropertyDescriptor = obj as FieldPropertyDescriptor;
			return fieldPropertyDescriptor != null && fieldPropertyDescriptor._field.Equals(this._field);
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00006432 File Offset: 0x00004632
		public override int GetHashCode()
		{
			return this._field.GetHashCode();
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000285 RID: 645 RVA: 0x0000643F File Offset: 0x0000463F
		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000286 RID: 646 RVA: 0x00006442 File Offset: 0x00004642
		public override void ResetValue(object component)
		{
		}

		// Token: 0x06000287 RID: 647 RVA: 0x00006444 File Offset: 0x00004644
		public override bool CanResetValue(object component)
		{
			return false;
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00006447 File Offset: 0x00004647
		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000289 RID: 649 RVA: 0x0000644A File Offset: 0x0000464A
		public override Type ComponentType
		{
			get
			{
				return this._field.DeclaringType;
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x0600028A RID: 650 RVA: 0x00006457 File Offset: 0x00004657
		public override Type PropertyType
		{
			get
			{
				return this._field.FieldType;
			}
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00006464 File Offset: 0x00004664
		public override object GetValue(object component)
		{
			return this._field.GetValue(component);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00006472 File Offset: 0x00004672
		public override void SetValue(object component, object value)
		{
			this._field.SetValue(component, value);
			this.OnValueChanged(component, EventArgs.Empty);
		}

		// Token: 0x04000212 RID: 530
		private readonly FieldInfo _field;
	}
}
