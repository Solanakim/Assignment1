using System.Web;
using System.Web.Optimization;

namespace HomePage
{
    public class BundleConfig
    {
        // 묶음에 대한 자세한 내용은 https://go.microsoft.com/fwlink/?LinkId=301862를 참조하세요.
        public static void RegisterBundles(BundleCollection bundles)
        {
            //제이쿼리 파일 추가
            bundles.Add(new ScriptBundle("~/Home/jquery").Include(
                 "~/Template/vendor/jquery/jquery.min.js",
                  "~/Template/Client-side-HTML-Table-Pagination-Plugin-with-jQuery-Paging/paging.js",
                   "~/Content/general.js"
                       //"~/Template/vendor/jquery/jquery.min.map",
                       //"~/Template/vendor/jquery/jquery.js",                      
                       //"~/Template/vendor/jquery/jquery.slim.js",
                       //"~/Template/vendor/jquery/jquery.slim.min.js",
                       // "~/Template/vendor/jquery/jquery.slim.min.map",
                       //   "~/Template/js/jqBootstrapValidation.js"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Modernizr의 개발 버전을 사용하여 개발하고 배우십시오. 그런 다음
            // 프로덕션에 사용할 준비를 하고 https://modernizr.com의 빌드 도구를 사용하여 필요한 테스트만 선택하세요.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //스크립트 파일 추가
            bundles.Add(new ScriptBundle("~/Home/bootstrap").Include(                
                  "~/Template/vendor/bootstrap/js/bootstrap.bundle.min.js"                  
                      //  "~/Template/vendor/bootstrap/js/bootstrap.bundle.min.js.map",
                      //"~/Template/vendor/bootstrap/js/bootstrap.bundle.js",
                      // "~/Template/vendor/bootstrap/js/bootstrap.bundle.js.map",                     
                      //  "~/Template/vendor/bootstrap/js/bootstrap.js",
                      //  "~/Template/vendor/bootstrap/js/bootstrap.js.map",
                      //   "~/Template/vendor/bootstrap/js/bootstrap.min.js",
                      //   "~/Template/vendor/bootstrap/js/bootstrap.min.js.map"
                      ));
            //스타일 시트 파일 추가
            bundles.Add(new StyleBundle("~/Home/css").Include(
                       //"~/Template/vendor/bootstrap/css/bootstrap-grid.css",
                       // "~/Template/vendor/bootstrap/css/bootstrap-grid.min.css",
                       //  "~/Template/vendor/bootstrap/css/bootstrap-reboot.css",
                       //   "~/Template/vendor/bootstrap/css/bootstrap-reboot.min.css",
                       //    "~/Template/vendor/bootstrap/css/bootstrap.css",
                       "~/Template/footable-bootstrap.latest/css/footable.bootstrap.css",
                           "~/Template/vendor/bootstrap/css/bootstrap.min.css",
                             "~/Template/css/modern-business.css"
                            


                           ));
        }
    }
}
