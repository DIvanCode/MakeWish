﻿using MakeWish.WishService.Models;

namespace MakeWish.WishService.Interfaces.DataAccess;

public interface IUsersRepository : IBaseRepository<User>
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<User>> GetUsersWithAccessToWishListAsync(WishList wishList, CancellationToken cancellationToken);
}