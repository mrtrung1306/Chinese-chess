using DemoQLDA.Hubs;
using DemoQLDA.Models;
using Libs;
using Libs.Entity;
using Libs.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DemoQLDA.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "DepartmentPolicy")]

    public class ChessController : ControllerBase
    {
        private IWebHostEnvironment hostEnvironment;
        private ChessService chessService;
        private IMemoryCache memoryCache;
        private IHubContext<ChatHub> hubContext;
        private CacheManages.CacheManage cachesManage;

        public ChessController(ChessService chessService, IMemoryCache memoryCache, IWebHostEnvironment hostEnvironment,IHubContext<ChatHub> hunContext)
        {
            this.hostEnvironment = hostEnvironment;
            this.chessService = chessService;
            this.memoryCache = memoryCache;
            this.hubContext = hunContext;
            this.cachesManage = new CacheManages.CacheManage(this.chessService, this.memoryCache);
        }
        [HttpPost]
        [Route("insertRoom")]
        //[Authorize(Policy = "DepartmentPolicy")]
        public IActionResult insertRoom()
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 1001);
            // Tạo tên phòng ngẫu nhiên dựa trên số ngẫu nhiên và thời gian hiện tại
            string Name = $"Room_{randomNumber}";
            Room usRoom = new Room();
            usRoom.Id = Guid.NewGuid();
            usRoom.Name = Name;
            // Chèn thông tin phòng vào cơ sở dữ liệu
            chessService.insertRoom(usRoom);
            return Ok(new { status = true, message = "",data = usRoom.Id });
        }
        [HttpGet]
        [Route("getRoom")]
        public IActionResult getRoom()
        {
            List<Room> roomList = chessService.getRoomList();
            return Ok(new { status = true, message = "", data = roomList });
        }
        [HttpPost]
        [Route("addUserToRoom")]
        public IActionResult addUserToRoom(Guid roomId) //user will get from entity sercurity
        {
            var userName = User.Claims.Where(x => x.Type == "name").FirstOrDefault()?.Value;
            UserInRoom usInRoom = new UserInRoom();
            usInRoom.Id = Guid.NewGuid();
            usInRoom.RoomId = roomId;
            usInRoom.UserName = userName;
            chessService.insertUserInRoom(usInRoom);
            if (!cachesManage.UserInRoom.ContainsKey(usInRoom.RoomId.ToString().ToLower()))
            {
                List<UserInRoom> userInRoomTemp = new List<UserInRoom>();
                userInRoomTemp.Add(usInRoom);
                cachesManage.UserInRoom.Add(usInRoom.RoomId.ToString().ToLower(), userInRoomTemp);
            }
            else
            {
                List<UserInRoom> userInRoomTemp = cachesManage.UserInRoom[usInRoom.RoomId.ToString().ToLower()];
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
        [HttpPost("removeUserInRoom")]
        public IActionResult RemoveUserInRoom()
        {
            var userName = User.Claims.Where(x => x.Type == "name").FirstOrDefault()?.Value;
            List<UserInRoom> roomId = chessService.getRoomInUserList(userName);
            Guid room = roomId[0].RoomId;
            chessService.RemoveUserinRoom(room, userName);
            return Ok(new { status = true, message = "" });
        }
        [HttpPost]
        [Route("moveChess")]
        public IActionResult moveChess([FromBody] MoveChessRequest request)
        {
            var userName = User.Claims.Where(x => x.Type == "name").FirstOrDefault()?.Value;
            List<UserInRoom> roomId = chessService.getRoomInUserList(userName);
            string room = roomId[0].RoomId.ToString().ToLower();
            List<UserInRoom> userInRoomList = cachesManage.UserInRoom[room.ToString().ToLower()];
            
            //if (!userInRoomList[0].UserName.Contains(userName))
            //{
            //    return Ok(new { status = false, message = "Bạn không có quyền di chuyển quân cờ trong phòng này." });
            //}
            hubContext.Groups.AddToGroupAsync(request.connect, room);
            hubContext.Clients.Groups(room).SendAsync("ReceiveChessMove", JsonSerializer.Serialize(request.moveNodeList));
            //hubContext.Clients.All.SendAsync("ReceiveChessMove", JsonSerializer.Serialize(request.moveNodeList));

            return Ok(new { status = true, message = "" });
        }

        [HttpGet]
        [Route("loadchessboard")]
        public IActionResult chessBoard()
        {
            string chessJson = System.IO.File.ReadAllText(hostEnvironment.ContentRootPath + "\\Data\\ChessJson.txt");
            List<ChessNode> chessNodeList = JsonSerializer.Deserialize<List<ChessNode>>(chessJson);
            List<List<PointModel>> matrix = new List<List<PointModel>>();
            for (int i = 0; i < 10; i++)
            {
                int top = 61 + i * 74;
                List<PointModel> pointList = new List<PointModel>();
                for (int j = 0; j < 9; j++)
                {
                    int left = 106 + j * 74;
                    PointModel p = new PointModel();
                    p.top = top;
                    p.left = left;
                    p.id = "";
                    ChessNode chessNode = chessNodeList.Where(s => s.top == top && s.left == left).FirstOrDefault();
                    if (chessNode != null)
                    {
                        p.id = chessNode.id;
                    }
                    pointList.Add(p);
                }
                matrix.Add(pointList);
            }
            return Ok(new { status = true, message = "", matrix = matrix, chessNode = chessNodeList });
        }       
    }
}
