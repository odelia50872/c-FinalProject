namespace GatherUp.Infrastructure.Services
{
    public static class EmailTemplates
    {
        public static string Build(string title, string preheader, string bodyContent)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<style>
  body {{ margin:0; padding:0; background:#f4f6fb; font-family:'Segoe UI',Arial,sans-serif; }}
  .wrapper {{ max-width:600px; margin:40px auto; background:white; border-radius:16px; overflow:hidden; box-shadow:0 4px 24px rgba(0,0,0,0.08); }}
  .header {{ background:linear-gradient(135deg,#5B6EF5,#8b5cf6); padding:32px 40px; text-align:center; }}
  .header h1 {{ color:white; margin:0 0 4px; font-size:24px; font-weight:800; letter-spacing:-0.5px; }}
  .header p {{ color:rgba(255,255,255,0.8); margin:0; font-size:14px; }}
  .body {{ padding:36px 40px; }}
  .body h2 {{ font-size:20px; color:#1e293b; margin:0 0 6px; font-weight:700; }}
  .preheader {{ font-size:14px; color:#64748b; margin-bottom:24px; }}
  .body p {{ font-size:15px; color:#334155; line-height:1.7; margin:0 0 16px; }}
  .info-box {{ background:#f8fafc; border:1px solid #e2e8f0; border-radius:12px; padding:20px 24px; margin:20px 0; }}
  .info-row {{ display:flex; gap:10px; margin-bottom:10px; font-size:14px; color:#334155; }}
  .info-row:last-child {{ margin-bottom:0; }}
  .info-label {{ font-weight:600; min-width:90px; color:#64748b; }}
  .cta-btn {{ display:inline-block; background:linear-gradient(135deg,#5B6EF5,#8b5cf6); color:white !important; text-decoration:none; padding:14px 32px; border-radius:10px; font-size:15px; font-weight:600; margin:20px 0; }}
  .footer {{ background:#f8fafc; border-top:1px solid #e2e8f0; padding:20px 40px; text-align:center; font-size:12px; color:#94a3b8; }}
</style>
</head>
<body>
<div class=""wrapper"">
  <div class=""header"">
    <h1>GatherUp</h1>
    <p>Event Management System</p>
  </div>
  <div class=""body"">
    <h2>{title}</h2>
    <p class=""preheader"">{preheader}</p>
    {bodyContent}
  </div>
  <div class=""footer"">
    <p>&copy; {DateTime.Now.Year} GatherUp &bull; Event Management System</p>
    <p>This email was sent automatically. Please do not reply.</p>
  </div>
</div>
</body>
</html>";
        }
    }
}
