using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
	public interface IConverterHelper
	{
		TeamEntity ToTeamEntity(TeamViewModel model, string path, bool isNew);

		TeamViewModel ToTeamViewModel(TeamEntity teamEntity);

	}
}
