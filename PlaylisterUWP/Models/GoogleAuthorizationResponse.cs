using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylisterUWP.Models
{
	public class GoogleAuthorizationResponse
	{
		public string State { get; set; }
		public string Code { get; set; }
	}
}
