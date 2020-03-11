using InfoEarthFrame.Application;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using InfoEarthFrame.DrawingOutput;
using InfoEarthFrame.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.EntityFramework;
using Abp.EntityFramework.Repositories;
using System.Web.Http;
using Infoearth.Application.Entity.DrawingOutput.Dtos;
using Infoearth.Util;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Collections;
using System.Web.Http.Description;

namespace InfoEarthFrame.WebApi.Next.Controllers
{
    public class DrawingOutputController : BaseApiController
    {
        private readonly IDrawingEntityRepository _drawingEntityRepository;
        public DrawingOutputController(IDrawingEntityRepository drawingEntityRepository)
        {
            this._drawingEntityRepository = drawingEntityRepository;
        }



        /// <summary>
        /// 根据制图ID获取制图信息
        /// </summary>
        /// <param name="drawId">制图ID</param>
        /// <returns></returns>
        public DrawingDTO GetDrawingInfoByTaskId(string drawId)
        {
            var db = (InfoEarthFrameDbContext)_drawingEntityRepository.GetDbContext();
            var entity = db.DrawingEntities.FirstOrDefault(p => p.Id == drawId);
            if (entity == null)
            {
                throw new NullReferenceException("为找到对应的制图任务ID");
            }

            DrawingDTO dto = new DrawingDTO()
            {
                ID = entity.Id,
                USERID = entity.USERID,
                BZ = entity.BZ,
                COMPLETE = entity.COMPLETE,
                CREATETIME = entity.CREATETIME,
                DRAWINGNAME = entity.DRAWINGNAME,
                ERRORMSG = entity.ERRORMSG,
                JD = entity.JD,
                OUTPUTPATH = entity.OUTPUTPATH,
                PARA = entity.PARA,
                STAUE = entity.STAUE,
                USERNAME = entity.USERNAME
            };
            return dto;
        }


        /// <summary>
        /// 更新制图信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public IHttpActionResult UpdataDrawingInfo([FromBody]DrawingUpdateInfoDTO info)
        {
            var db = (InfoEarthFrameDbContext)_drawingEntityRepository.GetDbContext();
            var entity = db.DrawingEntities.FirstOrDefault(p => p.Id == info.DrawingID);
            if (entity == null)
            {
                throw new NullReferenceException("为找到对应的制图任务ID");
            }
            if (!string.IsNullOrEmpty(info.COMPLETE))
            {
                entity.COMPLETE = info.COMPLETE;
            }
            if (!string.IsNullOrEmpty(info.ERRORMSG))
            {
                entity.ERRORMSG = info.ERRORMSG;
            }
            if (!string.IsNullOrEmpty(info.JD))
            {
                entity.JD = info.JD;
            }
            if (!string.IsNullOrEmpty(info.OUTPUTPATH))
            {
                entity.OUTPUTPATH = info.OUTPUTPATH;
            }
            if (!string.IsNullOrEmpty(info.STAUE))
            {
                entity.STAUE = info.STAUE;
            }

            db.Entry(entity).State = EntityState.Modified;
            var flag = db.SaveChanges() > 0;
            return Ok(GetResult(flag));
        }

    }
}
