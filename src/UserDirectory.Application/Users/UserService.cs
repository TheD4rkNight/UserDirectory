using UserDirectory.Application.Common;
using UserDirectory.Domain.Entities;

namespace UserDirectory.Application.Users;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetAllAsync(cancellationToken);
        return users.Select(ToResponse).ToList();
    }

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : ToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = new User(request.Name, request.Age!.Value, request.City, request.State, request.Pincode);
        await _repository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(user);
    }

    public async Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        if (user is null) return null;

        user.Update(request.Name, request.Age!.Value, request.City, request.State, request.Pincode);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(user);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        if (user is null) return false;

        _repository.Remove(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static UserResponse ToResponse(User user) =>
        new(user.Id, user.Name, user.Age, user.City, user.State, user.Pincode);
}
