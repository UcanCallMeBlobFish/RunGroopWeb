using RunGroopWeb.Data;

namespace RunGroopWeb.Repository;

public class ClubRepository : IClubRepository
{
    private readonly ApplicationDbContext _context;

    public ClubRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Club>> GetAll()
    {
        var clubs = await _context.Clubs.ToListAsync();
        return clubs;
    }

    public async Task<Club> GetByIdAsync(int id)
    {
        return await _context.Clubs.Include(i=> i.Address).FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Club>> GetClubByCity(string city)
    {
        return await _context.Clubs.Where(a => a.Address.City.Contains(city)).ToListAsync();
    }
    public async Task<Club> GetByIdAsyncNoTracking(int id)
    {
        return await _context.Clubs.Include(i=> i.Address).AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
    }
    
    public bool Add(Club club)
    {
        _context.Add(club);
        return Save();
    }

    public bool Delete(Club club)
    {
        _context.Remove(club);
        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0;
    }
    public bool Update(Club club)
    {
        _context.Update(club);
        return Save();
    }
}