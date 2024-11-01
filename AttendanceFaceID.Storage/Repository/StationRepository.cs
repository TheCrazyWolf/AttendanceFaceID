using AttendanceFaceID.Models.DLA;
using AttendanceFaceID.Storage.Context;
using Microsoft.EntityFrameworkCore;

namespace AttendanceFaceID.Storage.Repository;

public class StationRepository(AttendanceContext ef)
{
    public async Task<IList<Station>> GetStations()
    {
        return await ef.Stations.ToListAsync();
    }

    public async Task<Station?> GetStationById(long id)
    {
        return await ef.Stations.FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<Station?> GetStationByName(string name)
    {
        return await ef.Stations.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task AddStation(Station station)
    {
        await ef.Stations.AddAsync(station);
        await ef.SaveChangesAsync();
    }

    public async Task UpdateStation(Station station)
    {
        ef.Update(station);
        await ef.SaveChangesAsync();
    }

    public async Task DeleteStation(Station station)
    {
        ef.Stations.Remove(station);
        await ef.SaveChangesAsync();
    }
}