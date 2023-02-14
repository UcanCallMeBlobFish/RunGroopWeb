using Microsoft.AspNetCore.Mvc;
using RunGroopWeb.Data;
using RunGroopWeb.ViewModels;

namespace RunGroopWeb.Controllers;

public class RaceController : Controller
{
    private readonly IRaceRepository _raceRepository;
    private readonly IPotoService _potoService;

    public RaceController(IRaceRepository raceRepository, IPotoService potoService)
    {
        _raceRepository = raceRepository;
        _potoService = potoService;
    }

    public async Task<IActionResult> Index()
    {
        var races = await _raceRepository.GetAll();
        return View(races);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var races = await _raceRepository.GetByIdAsync(id);
        return View(races);
    }

    public IActionResult Create()
    {
        return View();
    }

    // [HttpPost]
    // public async Task<IActionResult> Create(Race race)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return View(race);
    //     }
    //
    //     _raceRepository.Add(race);
    //     return RedirectToAction("Index");
    // }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRaceViewModel clubVM)
    {
        if (!ModelState.IsValid)
        {
            var result = await _potoService.AddPhotoAsync(clubVM.Image);

            var club = new Race
            {
                Title = clubVM.Title,
                Description = clubVM.Description,
                Image = result.Url.ToString(),
                RaceCategory = clubVM.RaceCategory,
                AppUserId = clubVM.AppUserId,
                Address = new Address
                {
                    Street = clubVM.Address.Street,
                    City = clubVM.Address.City,
                    State = clubVM.Address.State,
                }
            };
            _raceRepository.Add(club);
            return RedirectToAction("Index");
        }
        else
        {
            ModelState.AddModelError("", "Photo upload failed");
        }

        return View(clubVM);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var race = await _raceRepository.GetByIdAsync(id);
        if (race == null) return View("Error");
        var raceVM = new EditRaceViewModel
        {
            Title = race.Title,
            Description = race.Description,
            AddressId = race.AddressId,
            Address = race.Address,
            URL = race.Image,
            RaceCategory = race.RaceCategory
        };
        return View(raceVM);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit club");
            return View(raceVM);
        }

        var userRace = await _raceRepository.GetByIdAsyncNoTracking(id);

        if (userRace == null)
        {
            return View("Error");
        }

        var photoResult = await _potoService.AddPhotoAsync(raceVM.Image);

        if (photoResult.Error != null)
        {
            ModelState.AddModelError("Image", "Photo upload failed");
            return View(raceVM);
        }

        if (!string.IsNullOrEmpty(userRace.Image))
        {
            _ = _potoService.DeletePhotoAsync(userRace.Image);
        }

        var race = new Race
        {
            Id = id,
            Title = raceVM.Title,
            Description = raceVM.Description,
            Image = photoResult.Url.ToString(),
            AddressId = raceVM.AddressId,
            Address = raceVM.Address,
        };

        _raceRepository.Update(race);

        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var clubDetails = await _raceRepository.GetByIdAsync(id);
        if (clubDetails == null) return View("Error");
        return View(clubDetails);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteClub(int id)
    {
        var raceDetails = await _raceRepository.GetByIdAsync(id);

        if (raceDetails == null)
        {
            return View("Error");
        }

        if (!string.IsNullOrEmpty(raceDetails.Image))
        {
            _ = _potoService.DeletePhotoAsync(raceDetails.Image);
        }

        _raceRepository.Delete(raceDetails);
        return RedirectToAction("Index");
    }
}
