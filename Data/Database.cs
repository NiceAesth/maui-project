using SQLite;
using PROIECT.Models;

namespace PROIECT.Data;

public class Database
{
    readonly SQLiteAsyncConnection _database;

    public Database(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);

        _database.CreateTableAsync<Apartament>().Wait();
        _database.CreateTableAsync<Locatar>().Wait();
        _database.CreateTableAsync<CheltuialaAsociatie>().Wait();
        _database.CreateTableAsync<FacturaIndividuala>().Wait();
        _database.CreateTableAsync<Sesizare>().Wait();
    }

    
    public Task<List<Apartament>> GetApartamenteAsync() => _database.Table<Apartament>().ToListAsync();

    public Task<int> SaveApartamentAsync(Apartament ap)
    {
        if (ap.ID != 0)
            return _database.UpdateAsync(ap);
        else
            return _database.InsertAsync(ap);
    }

    public Task<int> DeleteApartamentAsync(Apartament ap) => _database.DeleteAsync(ap);



    public Task<List<Locatar>> GetLocatariAsync() => _database.Table<Locatar>().ToListAsync();

    public Task<List<Locatar>> GetLocatariByApartamentAsync(int apartamentId)
        => _database.Table<Locatar>().Where(i => i.ApartamentID == apartamentId).ToListAsync();

    
    public Task<Locatar> LoginLocatarAsync(string email, string parola)
    {
    
        return _database.Table<Locatar>()
                        .Where(u => u.Email == email && u.Parola == parola)
                        .FirstOrDefaultAsync();
    }

   
    public Task<Locatar> GetLocatarAsync(int id)
    {
        return _database.Table<Locatar>()
                        .Where(i => i.ID == id)
                        .FirstOrDefaultAsync();
    }

    public Task<int> SaveLocatarAsync(Locatar locatar)
    {
        if (locatar.ID != 0)
            return _database.UpdateAsync(locatar);
        else
            return _database.InsertAsync(locatar);
    }

    public Task<int> DeleteLocatarAsync(Locatar item) => _database.DeleteAsync(item);


   
    public Task<List<FacturaIndividuala>> GetFacturiAsync() => _database.Table<FacturaIndividuala>().ToListAsync();

    public Task<List<FacturaIndividuala>> GetFacturiByApartamentAsync(int idAp)
        => _database.Table<FacturaIndividuala>().Where(i => i.IdApartament == idAp).ToListAsync();

    public Task<int> SaveFacturaAsync(FacturaIndividuala factura)
    {
        if (factura.ID != 0)
            return _database.UpdateAsync(factura);
        else
            return _database.InsertAsync(factura);
    }
    public Task<int> DeleteFacturaAsync(FacturaIndividuala factura)
    {
        return _database.DeleteAsync(factura);
    }



    public Task<List<Sesizare>> GetSesizariAsync() => _database.Table<Sesizare>().ToListAsync();

    public Task<int> SaveSesizareAsync(Sesizare s)
    {
        if (s.ID != 0)
            return _database.UpdateAsync(s);
        else
            return _database.InsertAsync(s);
    }

    public Task<int> DeleteSesizareAsync(Sesizare s) => _database.DeleteAsync(s);
}