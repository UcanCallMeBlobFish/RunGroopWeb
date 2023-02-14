using Microsoft.AspNetCore.Mvc;
using RunGroopWeb.Data;
using RunGroopWeb.ViewModels;

namespace RunGroopWeb.Controllers;

public class ClubController : Controller
{
    private readonly IClubRepository _clubRepository;
    private readonly IPotoService _potoService;

    public ClubController(IClubRepository clubRepository, IPotoService potoService)
    {
        _potoService = potoService;
        _clubRepository = clubRepository;
    }

    public async Task<IActionResult> Index()
    {
        var clubs = await _clubRepository.GetAll();
        return View(clubs);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var club = await _clubRepository.GetByIdAsync(id);
        return View(club);
    }

    public IActionResult Create()
    {
        return View();
    }
    // [HttpPost]
    // public async Task<IActionResult> Create(Club club)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return View(club);
    //     }
    //
    //     _clubRepository.Add(club);
    //     return RedirectToAction("Index");
    // }
    //

    [HttpPost]
    public async Task<IActionResult> Create(CreateClubViewModel clubVM)
    {
        if (!ModelState.IsValid)
        {
            var result = await _potoService.AddPhotoAsync(clubVM.Image);

            var club = new Club
            {
                Title = clubVM.Title,
                Description = clubVM.Description,
                Image = result.Url.ToString(),
                ClubCategory = clubVM.ClubCategory,
                AppUserId = clubVM.AppUserId,
                Address = new Address
                {
                    Street = clubVM.Address.Street,
                    City = clubVM.Address.City,
                    State = clubVM.Address.State,
                }
            };
            _clubRepository.Add(club);
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
        var club = await _clubRepository.GetByIdAsync(id);
        if (club == null) return View("Error");
        var clubVM = new EditClubViewModel
        {
            Title = club.Title,
            Description = club.Description,
            AddressId = club.AddressId,
            Address = club.Address,
            URL = club.Image,
            ClubCategory = club.ClubCategory
        };
        return View(clubVM);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit club");
            return View("Edit", clubVM);
        }

        var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);

        if (userClub == null)
        {
            return View("Error");
        }

        var photoResult = await _potoService.AddPhotoAsync(clubVM.Image);

        if (photoResult.Error != null)
        {
            ModelState.AddModelError("Image", "Photo upload failed");
            return View(clubVM);
        }

        if (!string.IsNullOrEmpty(userClub.Image))
        {
            _ = _potoService.DeletePhotoAsync(userClub.Image);
        }

        var club = new Club
        {
            Id = id,
            Title = clubVM.Title,
            Description = clubVM.Description,
            Image = photoResult.Url.ToString(),
            AddressId = clubVM.AddressId,
            Address = clubVM.Address,
        };

        _clubRepository.Update(club);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var clubDetails = await _clubRepository.GetByIdAsync(id);
        if (clubDetails == null) return View("Error");
        return View(clubDetails);
    }
    
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteClub(int id)
    {
        var clubDetails = await _clubRepository.GetByIdAsync(id);

        if (clubDetails == null)
        {
            return View("Error");
        }

        if (!string.IsNullOrEmpty(clubDetails.Image))
        {
            _ = _potoService.DeletePhotoAsync(clubDetails.Image);
        }

        _clubRepository.Delete(clubDetails);
        return RedirectToAction("Index");
    }
    
    
}