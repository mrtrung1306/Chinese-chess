using Libs.Entity;
using Libs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DemoQLDA.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChessController : ControllerBase
    {
        private ChessService chessService;
        private IMemoryCache memoryCache;
        private CacheManages.CacheManage cachesManage;

        public ChessController(ChessService chessService, IMemoryCache memoryCache)
        {
            this.chessService = chessService;
            this.memoryCache = memoryCache;
            this.cachesManage = new CacheManages.CacheManage(this.chessService, this.memoryCache);
        }
        [HttpPost]
        [Route("insertRoom")]
        public IActionResult insertRoom(string roomName)
        {
            Room room = new Room();
            room.Name = roomName;
            room.Id = Guid.NewGuid();
            chessService.insertRoom(room);
            return Ok(new { status = true, message = "" });
        }
        [HttpGet]
        [Route("getRoom")]
        public IActionResult insertRoom()
        {

            List<Room> roomList = chessService.getRoomList();
            return Ok(new { status = true, message = "", data = roomList });
        }
        [HttpPost]
        [Route("addUserToRoom")]
        public IActionResult addUserToRoom(Guid roomId, string userName) //user will get from entity sercurity
        {
            UserInRoom usInRoom = new UserInRoom();
            usInRoom.Id = Guid.NewGuid();
            usInRoom.RoomId = roomId;
            usInRoom.UserName = userName;

            chessService.insertUserInRoom(usInRoom);



            if (!cachesManage.UserInRoom.ContainsKey(roomId.ToString().ToLower()))
            {
                List<UserInRoom> userInRoomTemp = new List<UserInRoom>();
                userInRoomTemp.Add(usInRoom);
                cachesManage.UserInRoom.Add(roomId.ToString().ToLower(), userInRoomTemp);
            }
            else
            {
                List<UserInRoom> userInRoomTemp = cachesManage.UserInRoom[roomId.ToString().ToLower()];
                userInRoomTemp.Add(usInRoom);
            }
            return Ok(new { status = true, message = "" });
        }
        [HttpGet]
        [Route("getUserInRoom")]
        public IActionResult getUserInRoom(Guid roomId)
        {

            List<UserInRoom> userInRoomList = cachesManage.UserInRoom[roomId.ToString().ToLower()];// chessService.getUserInRoomList(roomId);
            return Ok(new { status = true, message = "", data = userInRoomList });
        }

    }
}
