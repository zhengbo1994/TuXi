using InfoEarthFrame.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InfoEarthFrame.DataManage.DTO
{
    public class UploadFileContext
    {
        public object Tag { get; set; }
        /// <summary>
        /// 是否是完整包上传
        /// </summary>
        public bool IsFullPackageUpload { get; set; }

        /// <summary>
        /// 主数据Id
        /// </summary>
        public string MainId { get; set; }

        /// <summary>
        /// 目录全名
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// 上传用户ID
        /// </summary>
        public string UploadUserId { get; set; }

        /// <summary>
        ///上传的文件
        /// </summary>
        public HttpPostedFile PostedFile { get; set; }

        public string PostedFileName
        {
            get
            {
                return PostedFile.FileName.ToLower();
            }
        }

        /// <summary>
        /// 上传文件保存的文件夹
        /// </summary>
        public string SaveDirectory
        {
            get
            {
                if (IsFullPackageUpload)
                {
                    return SaveFilePath;
                }
                return Path.GetDirectoryName(SaveFilePath);
            }
        }
        /// <summary>
        /// 上传文件临时保存路径
        /// </summary>
        public string SaveFilePath
        {
            get;
            set;
        }


        protected virtual List<FileInfo> ConvertedFiles
        {
            get
            {
                return new List<FileInfo>{
                  new FileInfo(SaveFilePath)
                 };
            }
        }

        /// <summary>
        /// 最终文件
        /// </summary>
        public List<UploadFileDto> Files
        {
            get
            {
                return ConvertedFiles.Select(p => new UploadFileDto
                {
                    Ext = p.Extension,
                    FileData = FtpWorkingDirectory + "\\" + p.Name,
                    MainID = MainId,
                    FileName = p.Name,
                    Id = Guid.NewGuid().ToString(),
                    FileSize = p.Length,
                    FolderName = FolderName
                }).ToList();
            }
        }




        public string FtpName
        {
            get
            {
                return "Package";
            }
        }

        public int ErrorCode { get; protected set; }

        private List<string> _errorInfo;
        public List<string> ErrorInfo
        {
            get
            {
                if (_errorInfo == null)
                {
                    _errorInfo = new List<string>();
                }
                return _errorInfo;
            }
            private set
            {
                _errorInfo = value;
            }
        }

        public string FtpWorkingDirectory
        {
            get
            {
                return "Package\\" + MainId + "\\" + FolderName;
            }
        }

        protected virtual bool IsRenamePostedFile
        {
            get
            {
                return false;
            }
        }


     /// <summary>
        /// 将文件保存到临时文件夹
     /// </summary>
        private void SaveFile()
        {
            //不是整个包上传
            if (!IsFullPackageUpload)
            {
                var tempDir = Path.Combine(HttpContext.Current.Server.MapPath("~"), ConfigContext.Current.DefaultConfig["upload:tempdir"], MainId, FolderName);
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
            
                }
                Directory.CreateDirectory(tempDir);
               
                if (IsRenamePostedFile)
                {
                    var newFileName = PostedFileName.Insert(PostedFileName.LastIndexOf("."), "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    SaveFilePath = Path.Combine(tempDir, newFileName);
                }
                else
                {
                    SaveFilePath = Path.Combine(tempDir, PostedFile.FileName);
                }
                PostedFile.SaveAs(SaveFilePath);
            }
            else
            {

            }

      
        }

        private void MakeThumbnail(string imgPath_old, string imgPath_new, int width, int height)
        {

            using (System.Drawing.Image img = System.Drawing.Image.FromStream(new System.IO.MemoryStream(System.IO.File.ReadAllBytes(imgPath_old))))
            {

                int towidth = width; int toheight = height;
                int x = 0; int y = 0; int ow = img.Width;
                int oh = img.Height;
                // 按值较大的进行等比缩放（不变形） 
                if ((double)img.Width / (double)towidth < (double)img.Height / (double)toheight)
                {
                    toheight = height;
                    towidth = img.Width * height / img.Height;
                }
                else
                {
                    towidth = width;
                    toheight = img.Height * width / img.Width;
                }
                //新建一个bmp图片
                using (System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight))
                {
                    //新建一个画板
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        //设置高质量插值法
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                        //设置高质量,低速度呈现平滑程度
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        //清空画布并以透明背景色填充
                        g.Clear(System.Drawing.Color.Transparent);
                        //在指定位置并且按指定大小绘制原图片的指定部分
                        g.DrawImage(img, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                        new System.Drawing.Rectangle(x, y, ow, oh),
                        System.Drawing.GraphicsUnit.Pixel);

                        bitmap.Save(imgPath_new, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
        }

        /// <summary>
        /// 上传文件之前
        /// </summary>
        protected virtual void BeforeUploadToFtp()
        {
         
        }

       


        /// <summary>
        /// 上传文件之后
        /// </summary>
        protected virtual void AfterUploadToFtp()
        {
            if (FolderName.Contains("栅格图"))
            {
                var ftpFilePPath=Path.Combine(ConfigContext.Current.FtpConfig[FtpName].DirectoryPath,Files[0].FileData);
                var thumbFilePath = ftpFilePPath.Insert(ftpFilePPath.LastIndexOf("."), "_thumb");
                MakeThumbnail(SaveFilePath, thumbFilePath, 220, 200);
            }
        }

        public void RemovePostedFiles()
        {
            //单个上传的话删除自身文件
            if (!IsFullPackageUpload)
            {
                var dir = new DirectoryInfo(SaveDirectory);
                foreach (var file in dir.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                //try
                //{
                //    File.Delete(SaveFilePath);
                //}
                //catch 
                //{ 
                
                //}
            }
        }
        public  void Upload()
        {
            try
            {
                this.SaveFile();

                this.BeforeUploadToFtp();
                if (ErrorInfo.Count == 0)
                {
                    this.UploadToFtp();
                    ErrorCode = 0;

                    this.AfterUploadToFtp();

                    if (ErrorInfo.Count > 0)
                    {
                        ErrorCode = -200;
                    }
                }
                else
                {
                    ErrorCode = -200;
                }
            }
            catch (Exception ex)
            {
                ErrorCode = 500;
                ErrorInfo.Add(ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 上传文件到FTP
        /// </summary>
        protected   void UploadToFtp()
        {
  
            //先获取FTP
            var ftp = ConfigContext.Current.FtpConfig[FtpName];
            if (ftp == null)
            {
                //FTP配置不存在
                ErrorCode = -503;
                return;
            }


            //连接FTP
            using (var client = new FtpHelper(ftp))
            {
                client.UploadFiles(ConvertedFiles.Select(p => p.FullName), FtpWorkingDirectory);
            }
        }
    }
}
