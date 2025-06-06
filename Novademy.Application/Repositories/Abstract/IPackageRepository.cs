using Microsoft.AspNetCore.Http;
using Novademy.Application.Models;

namespace Novademy.Application.Repositories.Abstract;

public interface IPackageRepository
{
    Task<Package> CreatePackageAsync(Package package, IFormFile? image);
    Task<IEnumerable<Package>> GetAllPackagesAsync();
    Task<Package?> GetPackageByIdAsync(Guid id);
    Task<Package?> UpdatePackageAsync(Package package, IFormFile? image);
    Task DeletePackageAsync(Guid id);
}