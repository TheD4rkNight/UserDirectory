using Microsoft.EntityFrameworkCore;
using UserDirectory.Application.Users;
using UserDirectory.Domain.Entities;
using UserDirectory.Infrastructure.Persistence;

namespace UserDirectory.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Users.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        _db.Users.AddAsync(user, cancellationToken).AsTask();

    public void Remove(User user) => _db.Users.Remove(user);
}
