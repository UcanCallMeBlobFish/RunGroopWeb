using RunGroopWeb.Data;

namespace RunGroopWeb.Repository;

public class RaceRepository : IRaceRepository
{
    private readonly ApplicationDbContext _context;

    public RaceRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Race>> GetAll()
    {
        return await _context.Races.ToListAsync();
    }

    public async Task<Race> GetByIdAsync(int id)
    {
        return await _context.Races.Include(i => i.Address).Where(a => a.Id == id).FirstOrDefaultAsync();
    }
    public async Task<Race> GetByIdAsyncNoTracking(int id)
    {
        return await _context.Races.Include(i => i.Address).AsNoTracking().Where(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Race>> GetAllRacesByCity(string city)
    {
        return await _context.Races.Where(a => a.Address.City.Contains(city)).ToListAsync();
    }

    public bool Add(Race race)
    {
        _context.Add(race);
        return Save();
    }

    public bool Delete(Race race)
    {
        _context.Remove(race);
        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0;
    }

    public bool Update(Race race)
    {
        _context.Update(race);
        return Save();
    }
}