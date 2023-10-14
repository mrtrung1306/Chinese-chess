using DemoQLDA.Hubs;
using DemoQLDA.Models;
using Libs.Entity;
using Libs.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace DemoQLDA.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "DepartmentPolicy")]

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
        public IActionResult getRoom()
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
        [HttpPost]
        [Route("moveChess")]
        public IActionResult moveChess(List<MoveChess> moveNodeList)
        {
            hubContext.Clients.All.SendAsync("ReceiveChessMove",JsonSerializer.Serialize(moveNodeList));
            return Ok(new { status = true, message = "" });
        }
    }
}
