//------------------------------------------------------------------------------
// <auto-generated>
//    이 코드는 템플릿에서 생성되었습니다.
//
//    이 파일을 수동으로 변경하면 응용 프로그램에 예기치 않은 동작이 발생할 수 있습니다.
//    코드가 다시 생성되면 이 파일에 대한 수동 변경 사항을 덮어씁니다.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HomePage.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ArticleCategories
    {
        public int ArticleIDX { get; set; }
        public int CategoryCode { get; set; }
        public Nullable<System.DateTime> RegstDate { get; set; }
    
        public virtual Categories Categories { get; set; }
        public virtual Articles Articles { get; set; }
    }
}
