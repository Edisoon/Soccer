﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;
using Soccer.Web.Helpers;
using Soccer.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Soccer.Web.Controllers
{
	public class TournamentsController : Controller
	{
		private readonly DataContext _context;
		private readonly IImageHelper _imageHelper;
		private readonly IConverterHelper _converterHelper;

		public TournamentsController(
			DataContext context,
			IImageHelper imageHelper,
			IConverterHelper converterHelper)
		{
			_context = context;
			_imageHelper = imageHelper;
			_converterHelper = converterHelper;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context
				.Tournaments
				.Include(t => t.Groups)
				.OrderBy(t => t.StartDate)
				.ToListAsync());
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(TournamentViewModel model)
		{
			if (ModelState.IsValid)
			{
				string path = string.Empty;

				if (model.LogoFile != null)
				{
					path = await _imageHelper.UploadImageAsync(model.LogoFile, "Tournaments");
				}
				if (model.EndDate.Date <= model.StartDate.Date)
				{
					ModelState.AddModelError(string.Empty, "The end date cannot be less than the start date");
				}
				TournamentEntity tournament = _converterHelper.ToTournamentEntity(model, path, true);
				_context.Add(tournament);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			return View(model);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			TournamentEntity tournamentEntity = await _context.Tournaments.FindAsync(id);
			if (tournamentEntity == null)
			{
				return NotFound();
			}

			TournamentViewModel model = _converterHelper.ToTournamentViewModel(tournamentEntity);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(TournamentViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (ModelState.IsValid)
				{
					string path = model.LogoPath;

					if (model.LogoFile != null)
					{
						path = await _imageHelper.UploadImageAsync(model.LogoFile, "Tournaments");
					}

					if (model.EndDate.Date <= model.StartDate.Date)
					{
						ModelState.AddModelError(string.Empty, "The end date cannot be less than the start date");
					}

					TournamentEntity tournamentEntity = _converterHelper.ToTournamentEntity(model, path, false);
					_context.Update(tournamentEntity);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
			}

			return View(model);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			TournamentEntity tournamentEntity = await _context.Tournaments
				.FirstOrDefaultAsync(m => m.Id == id);
			if (tournamentEntity == null)
			{
				return NotFound();
			}

			_context.Tournaments.Remove(tournamentEntity);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			TournamentEntity tournamentEntity = await _context.Tournaments
				.Include(t => t.Groups)
				.ThenInclude(t => t.Matches)
				.ThenInclude(t => t.Local)
				.Include(t => t.Groups)
				.ThenInclude(t => t.Matches)
				.ThenInclude(t => t.Visitor)
				.Include(t => t.Groups)
				.ThenInclude(t => t.GroupDetails)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (tournamentEntity == null)
			{
				return NotFound();
			}

			return View(tournamentEntity);
		}

		public async Task<IActionResult> AddGroup(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			TournamentEntity tournamentEntity = await _context.Tournaments.FindAsync(id);
			if (tournamentEntity == null)
			{
				return NotFound();
			}

			GroupViewModel model = new GroupViewModel
			{
				Tournament = tournamentEntity,
				TournamentId = tournamentEntity.Id
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddGroup(GroupViewModel model)
		{
			if (ModelState.IsValid)
			{
				GroupEntity groupEntity = await _converterHelper.ToGroupEntityAsync(model, true);
				_context.Add(groupEntity);
				await _context.SaveChangesAsync();
				return RedirectToAction($"{nameof(Details)}/{model.TournamentId}");
			}

			return View(model);
		}

		public async Task<IActionResult> EditGroup(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			GroupEntity groupEntity = await _context.Groups
				.Include(g => g.Tournament)
				.FirstOrDefaultAsync(g => g.Id == id);
			if (groupEntity == null)
			{
				return NotFound();
			}

			GroupViewModel model = _converterHelper.ToGroupViewModel(groupEntity);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditGroup(GroupViewModel model)
		{
			if (ModelState.IsValid)
			{
				GroupEntity groupEntity = await _converterHelper.ToGroupEntityAsync(model, false);
				_context.Update(groupEntity);
				await _context.SaveChangesAsync();
				return RedirectToAction($"{nameof(Details)}/{model.TournamentId}");
			}

			return View(model);
		}

		public async Task<IActionResult> DeleteGroup(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var groupEntity = await _context.Groups
				.Include(g => g.Tournament)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (groupEntity == null)
			{
				return NotFound();
			}

			_context.Groups.Remove(groupEntity);
			await _context.SaveChangesAsync();
			return RedirectToAction($"{nameof(Details)}/{groupEntity.Tournament.Id}");
		}
		public async Task<IActionResult> DetailsGroup(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var groupEntity = await _context.Groups
				.Include(g => g.Matches)
				.ThenInclude(g => g.Local)
				.Include(g => g.Matches)
				.ThenInclude(g => g.Visitor)
				.Include(g => g.Tournament)
				.Include(g => g.GroupDetails)
				.ThenInclude(gd => gd.Team)
				.FirstOrDefaultAsync(g => g.Id == id);
			if (groupEntity == null)
			{
				return NotFound();
			}

			return View(groupEntity);
		}


	}
}
