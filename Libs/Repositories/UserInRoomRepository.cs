using Azure.Core;
using Libs.Data;
using Libs.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Repositories
{
    public interface IUserInRoomRepository : IRepository<UserInRoom>
    {
        public void insertUserInRoom(UserInRoom UserInRoom);
        public List<UserInRoom> getUserInRoomList(Guid roomId);
        public List<UserInRoom> getUserInRoomList();
        public List<UserInRoom> getRoomInUserList(string userName);
        public void DeleteUserInRoomByName(Guid roomId, string userName);
    }
    public class UserInRoomRepository : RepositoryBase<UserInRoom>, IUserInRoomRepository
    {
        public UserInRoomRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public void insertUserInRoom(UserInRoom UserInRoom)
        {
            _dbContext.UserInRoom.Add(UserInRoom);
        }
        public List<UserInRoom> getUserInRoomList(Guid roomId)
        {
            return _dbContext.UserInRoom.Where(s => s.RoomId == roomId).ToList();
        }
        public List<UserInRoom> getRoomInUserList(string userName)
        {
            return _dbContext.UserInRoom.Where(s => s.UserName == userName).ToList();
        }
        public List<UserInRoom> getUserInRoomList()
        {
            return _dbContext.UserInRoom.ToList();
        }
        public void DeleteUserInRoomByName(Guid roomId,string userName)
        {
            var userToDelete = _dbContext.UserInRoom.FirstOrDefault(u => u.RoomId == roomId && u.UserName == userName);
            
            if (userToDelete != null)
            {
                _dbContext.UserInRoom.RemoveRange(userToDelete);
                _dbContext.SaveChanges();
                var userInRoom = _dbContext.UserInRoom.Where(s => s.RoomId == roomId).ToList();
                if (userInRoom.Count == 0)
                {
                    var roomToDelete = _dbContext.Room.FirstOrDefault(r => r.Id == roomId);
                    if (roomToDelete != null)
                    {
                        _dbContext.Room.Remove(roomToDelete);
                    }
                }
            }
        }
    }
}
