using System;
using System.Collections.Generic;
using Model.Events;

namespace Model.DataService
{
	// Token: 0x02000078 RID: 120
	public class EventLogQueryResult
	{
		// Token: 0x06000335 RID: 821 RVA: 0x00008F26 File Offset: 0x00007126
		public IList<SentinelState> GetStateList()
		{
			return this._stateList;
		}

		// Token: 0x06000336 RID: 822 RVA: 0x00008F2E File Offset: 0x0000712E
		public void Add(string name, DateTime dt, string currentState, string priorState, string comment)
		{
			this._stateList.Add(new SentinelState(name, dt, currentState, priorState, comment));
		}

		// Token: 0x04000247 RID: 583
		private readonly List<SentinelState> _stateList = new List<SentinelState>();
	}
}
