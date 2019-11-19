using RIPP.NIR;
using RIPP.NIR.Models;
using RIPP.Web.Chem.Datas;
using RIPP.Web.Chem.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace RIPP.Web.Chem.Upload
{
    /// <summary>
    /// Summary description for UploadHandler
    /// </summary>
    public class UploadHandler : IHttpHandler
    {
        private readonly JavaScriptSerializer js;

        private string StorageRoot
        {
            get { return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/")); } //Path should! always end with '/'
        }

        public UploadHandler()
        {
            js = new JavaScriptSerializer();
            js.MaxJsonLength = 41943040;
        }

        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            HandleMethod(context);
        }

        // Handle request based on method
        private void HandleMethod(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                    if (GivenFilename(context)) DeliverFile(context);
                    else ListCurrentFiles(context);
                    break;
                case "POST":
                case "PUT":
                    UploadFile(context);
                    break;

                case "DELETE":
                    DeleteFile(context);
                    break;

                case "OPTIONS":
                    ReturnOptions(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }

        private static void ReturnOptions(HttpContext context)
        {
            context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
            context.Response.StatusCode = 200;
        }

        // Delete file from the server
        private void DeleteFile(HttpContext context)
        {

            var type = context.Request["t"];
            var id = Convert.ToInt32(context.Request["id"]);
            using (var db = new RIPPWebEntities())
            {
                string sql = "", filePath = "";
                switch (type)
                {
                    case "m"://删除模型
                        var m = db.model.Where(d => d.id == id).FirstOrDefault();
                        if (m != null)//删除文件
                        {
                            filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/"), m.path);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                        //先删除模型所预测过的结果
                        sql = string.Format("delete from results where mid={0}", id);
                        db.Database.ExecuteSqlCommand(sql);
                        sql = string.Format("delete from model where id={0}", id);
                        db.Database.ExecuteSqlCommand(sql);
                        break;
                    case "s":
                        var s = db.spec.Where(d => d.id == id).FirstOrDefault();
                        if (s != null)//删除文件
                        {
                            filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/"), s.path);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                        //先删除模型所预测过的结果
                        sql = string.Format("delete from results where sid={0}", id);
                        db.Database.ExecuteSqlCommand(sql);
                        sql = string.Format("delete from spec where id={0}", id);
                        db.Database.ExecuteSqlCommand(sql);
                        break;
                    default:
                        break;
                }

                context.Response.AddHeader("Vary", "Accept");
                try
                {
                    if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                        context.Response.ContentType = "application/json";
                    else
                        context.Response.ContentType = "text/plain";
                }
                catch
                {
                    context.Response.ContentType = "text/plain";
                }



                context.Response.Write("{\"success\":true}");

            }

        }

        // Upload file to the server
        private void UploadFile(HttpContext context)
        {
            var statuses = new Files();

            statuses.files = new List<FilesStatus>();
            var headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses.files);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses.files);
            }

            WriteJsonIframeSafe(context, statuses);
        }

        private FilesStatus upload(Stream inputStream, string fileName, HttpContext context)
        {
            var statu = new FilesStatus();

            var filename = RIPP.Web.Chem.Tools.Common.GetRnd(10, true, true, false) + Path.GetExtension(fileName);
            var tempfullName = Path.Combine(StorageRoot, filename);
            statu.progress = "1.0";
            statu.size = (int)inputStream.Length;
            statu.name = fileName;
            try
            {
                //先保存在服务器一个临时文件
                using (var fs = new FileStream(tempfullName, FileMode.Append, FileAccess.Write))
                {
                    var buffer = new byte[1024];
                    var l = inputStream.Read(buffer, 0, 1024);
                    while (l > 0)
                    {
                        fs.Write(buffer, 0, l);
                        l = inputStream.Read(buffer, 0, 1024);
                    }
                    fs.Flush();
                    fs.Close();
                }
            }
            catch
            {
                statu.error = "服务器错误，上传文件失败!";
                return statu;
            }

            //根据文件后缀尝试打开文件
            var ftype = BindModel.CheckType(tempfullName);
            model dbmodel = new model();
            spec sp = new spec();
            try
            {
                statu.type = ftype.GetDescription();
                dbmodel.path = Path.Combine(RIPP.Web.Chem.Tools.Common.GetUploadPath(), filename);
                sp.path = dbmodel.path;
                switch (ftype)
                {
                    case FileExtensionEnum.Allmethods:
                        var mBind = BindModel.ReadModel<BindModel>(tempfullName);
                        statu.description = mBind.ToString();
                        dbmodel.createtime = mBind.CreateTime;
                        dbmodel.name = mBind.Name;
                        // ;
                        // dbmodel.type = (int)ftype;
                        //  dbmodel.
                        break;
                    case FileExtensionEnum.PLSBind:
                        var mPLS = BindModel.ReadModel<PLSModel>(tempfullName);
                        statu.description = mPLS.ToString();
                        dbmodel.createtime = mPLS.CreateTime;
                        dbmodel.name = mPLS.Name;
                        break;
                    case FileExtensionEnum.IdLib:
                        var mId = BindModel.ReadModel<IdentifyModel>(tempfullName);
                        statu.description = mId.ToString();
                        dbmodel.createtime = mId.CreateTime;
                        dbmodel.name = mId.Name;
                        break;
                    case FileExtensionEnum.FitLib:
                        var mFitting = BindModel.ReadModel<FittingModel>(tempfullName);
                        statu.description = mFitting.ToString();
                        dbmodel.createtime = mFitting.CreateTime;
                        dbmodel.name = mFitting.Name;
                        break;
                    case FileExtensionEnum.PLS1:
                    case FileExtensionEnum.PLSANN:
                        var mPLS1 = BindModel.ReadModel<PLSSubModel>(tempfullName);
                        statu.description = mPLS1.ToString();
                        dbmodel.createtime = mPLS1.CreateTime;
                        dbmodel.name = mPLS1.Name;
                        break;
                    case FileExtensionEnum.ItgBind:
                        var itgSub = BindModel.ReadModel<IntegrateModel>(tempfullName);
                        statu.description = itgSub.ToString();
                        dbmodel.createtime = itgSub.CreateTime;
                        dbmodel.name = itgSub.Name;
                        break;
                    case FileExtensionEnum.Spec:
                    default:
                        var s = new Spectrum(tempfullName);
                        ftype = FileExtensionEnum.Spec;
                        sp.Spec = s;
                        sp.name = Path.GetFileName(fileName);
                        statu.description = "光谱文件";
                        break;
                        
                }
            }
            catch (Exception ex)//读取文件失败
            {
                statu.error = "上传的文件类型有误，服务器无法识别，上传文件失败!";
             
                if (File.Exists(tempfullName))
                {
                    File.Delete(tempfullName);
                }
                return statu;
            }


            //根据文件类型保存数据库
            using (var db = new RIPPWebEntities())
            {
                if (ftype != FileExtensionEnum.Unkown)
                {
                    var user = RIPP.Web.Chem.Tools.Common.Get_User;
                    if (user == null)
                    {
                        var uid = Convert.ToInt32(context.Request["uid"]);
                        user = db.S_User.Where(d => d.ID == uid).FirstOrDefault();
                    }

                    if (ftype == FileExtensionEnum.Spec)//光谱
                    {
                        if (RIPP.Web.Chem.Tools.Common.UserCanPredict(user))
                        {

                            sp.addtime = DateTime.Now;
                            sp.ext = Path.GetExtension(fileName).Substring(1);
                            sp.uid = user.ID;
                            sp.gid = user.GroupID;
                            db.spec.Add(sp);
                            db.SaveChanges();
                            statu.deleteType = "DELETE";
                            statu.deleteUrl = "/upload/UploadHandler.ashx?t=s&id=" + sp.id;
                            statu.url = "/admin/spec/detail/" + sp.id;
                            statu.id = sp.id;

                            File.Move(tempfullName, Path.Combine(HttpContext.Current.Server.MapPath("~/"), sp.path));//移动刚才上传的临时文件
                        }
                        else
                        {
                            statu.error = "您无权上传光谱文件，请与管理员联系。";
                            if (File.Exists(tempfullName))
                            {
                                File.Delete(tempfullName);
                            }
                        }
                    }
                    else//模型
                    {
                        if (user.HasRole(RoleEnum.Administrator) || user.HasRole(RoleEnum.Engineer))
                        {
                            dbmodel.addtime = DateTime.Now;
                            dbmodel.type = (int)ftype;
                            dbmodel.uid = user.ID;
                            dbmodel.gid = user.GroupID;
                            db.model.Add(dbmodel);
                            db.SaveChanges();
                            statu.deleteType = "DELETE";
                            statu.deleteUrl = "/upload/UploadHandler.ashx?t=m&id=" + dbmodel.id;
                            statu.url = "/admin/model/detail/" + dbmodel.id;
                            statu.id = dbmodel.id;
                            File.Move(tempfullName, Path.Combine(HttpContext.Current.Server.MapPath("~/"), dbmodel.path));//移动刚才上传的临时文件
                        }
                        else
                        {
                            statu.error = "您无权上传模型文件，请与管理员联系。";
                            if (File.Exists(tempfullName))
                            {
                                File.Delete(tempfullName);
                            }
                        }
                    }

                }
            }

            return statu;
        }

        // Upload partial file
        private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses)
        {
            if (context.Request.Files.Count != 1)
                throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            statuses.Add(upload(context.Request.Files[0].InputStream, context.Request.Files[0].FileName, context));
        }

        // Upload entire file
        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                statuses.Add(upload(context.Request.Files[i].InputStream, context.Request.Files[i].FileName, context));
            }
        }

        private void WriteJsonIframeSafe(HttpContext context, Files statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                    context.Response.ContentType = "application/json";
                else
                    context.Response.ContentType = "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }



            var jsonObj = js.Serialize(statuses);
            context.Response.Write(jsonObj);
        }

        private static bool GivenFilename(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Request["f"]);
        }

        private void DeliverFile(HttpContext context)
        {
            var type = context.Request["t"];
            var id = Convert.ToInt32(context.Request["id"]);
            using (var db = new RIPPWebEntities())
            {
                string path = "";
                switch (type)
                {
                    case "m":
                        var model = db.model.Where(d => d.id == id).FirstOrDefault();
                        path = model != null ? model.path : null;
                        break;
                    case "s":
                        var spec = db.spec.Where(d => d.id == id).FirstOrDefault();
                        path = spec != null ? spec.path : null;
                        break;
                    default:
                        break;
                }
                var filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/"), path);
                if (File.Exists(filePath))
                {
                    context.Response.AddHeader("Content-Disposition", "attachment; ");
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.ClearContent();
                    context.Response.WriteFile(filePath);
                }
                else
                    context.Response.StatusCode = 404;
            }


            
        }

        private void ListCurrentFiles(HttpContext context)
        {
            //var files =
            //    new DirectoryInfo(StorageRoot)
            //        .GetFiles("*", SearchOption.TopDirectoryOnly)
            //        .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
            //        .Select(f => new FilesStatus(f))
            //        .ToArray();

            //string jsonObj = js.Serialize(files);
            //context.Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
            //context.Response.Write(jsonObj);
            //context.Response.ContentType = "application/json";
        }
    }
}