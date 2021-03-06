﻿using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
	public class ConverterHelper : IConverterHelper
	{
		private readonly DataContext _context;

		public ConverterHelper(DataContext context)
		{
			_context = context;
		}

		public TeamEntity ToTeamEntity(TeamViewModel model, string path, bool isNew)
		{
			return new TeamEntity
			{
				Id = isNew ? 0 : model.Id,
				LogoPath = path,
				Name = model.Name
			};
		}

		public TeamViewModel ToTeamViewModel(TeamEntity teamEntity)
		{
			return new TeamViewModel
			{
				Id = teamEntity.Id,
				LogoPath = teamEntity.LogoPath,
				Name = teamEntity.Name
			};
		}

		public TournamentEntity ToTournamentEntity(TournamentViewModel model, string path, bool isNew)
		{
			return new TournamentEntity
			{
				EndDate = model.EndDate.ToUniversalTime(),
				Groups = model.Groups,
				Id = isNew ? 0 : model.Id,
				IsActive = model.IsActive,
				LogoPath = path,
				Name = model.Name,
				StartDate = model.StartDate.ToUniversalTime()
			};
		}

		public TournamentViewModel ToTournamentViewModel(TournamentEntity tournamentEntity)
		{
			return new TournamentViewModel
			{
				EndDate = tournamentEntity.EndDate,
				Groups = tournamentEntity.Groups,
				Id = tournamentEntity.Id,
				IsActive = tournamentEntity.IsActive,
				LogoPath = tournamentEntity.LogoPath,
				Name = tournamentEntity.Name,
				StartDate = tournamentEntity.StartDate
			};
		}

		public async Task<GroupEntity> ToGroupEntityAsync(GroupViewModel model, bool isNew)
		{
			return new GroupEntity
			{
				GroupDetails = model.GroupDetails,
				Id = isNew ? 0 : model.Id,
				Matches = model.Matches,
				Name = model.Name,
				Tournament = await _context.Tournaments.FindAsync(model.TournamentId)
			};
		}

		public GroupViewModel ToGroupViewModel(GroupEntity groupEntity)
		{
			return new GroupViewModel
			{
				GroupDetails = groupEntity.GroupDetails,
				Id = groupEntity.Id,
				Matches = groupEntity.Matches,
				Name = groupEntity.Name,
				Tournament = groupEntity.Tournament,
				TournamentId = groupEntity.Tournament.Id
			};
		}

	}
}
