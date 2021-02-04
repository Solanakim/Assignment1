//파라미터 가져오기
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"), results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

// 쿠키 생성 함수
function setCookie(cName, cValue, cDay) {
    var expire = new Date();
    expire.setDate(expire.getDate() + cDay);
    cookies = cName + '=' + escape(cValue) + '; path=/ '; // 한글 깨짐을 막기위해 escape(cValue)를 합니다.
    if (typeof cDay != 'undefined') cookies += ';expires=' + expire.toGMTString() + ';';
    document.cookie = cookies;
}



// 쿠키 가져오기 함수
function getCookie(cName) {
    cName = cName + '=';
    var cookieData = document.cookie;
    var start = cookieData.indexOf(cName);
    var cValue = '';
    if (start != -1) {
        start += cName.length;
        var end = cookieData.indexOf(';', start);
        if (end == -1) end = cookieData.length;
        cValue = cookieData.substring(start, end);
    }
    return unescape(cValue);
}

//쿠키 삭제
function deleteCookie(cookieName) {
    var expireDate = new Date();
    expireDate.setDate(expireDate.getDate() - 1);
    document.cookie = cookieName + "= " + "; expires=" + expireDate.toGMTString();
}


    let logoutUser = false;
    let timeoutHnd = null;
    let logouTimeInterval = 20 * 60 * 1000; // 15 mins here u can increase session time
 
    function onuser_activite() {       
        if (logoutUser) {
            ;
        }
        else {
            ResetLogOutTimer();
        }
    }
    function OnTimeoutReached() {
        logoutUser = true;
        swal({
            title: "세션 기간 만료로 로그아웃 되었습니다. 다시 로그인 해주세요.",
            //text: "Check ID and PW again.",
            icon: "warning",
        }).then((result) => {
            logout();
        });
    }
    function ResetLogOutTimer() {
        clearTimeout(timeoutHnd);
        // set new timer
        timeoutHnd = setTimeout('OnTimeoutReached();', logouTimeInterval);
    }    

    function sessionStart() {
        //document.body.onclick = onuser_activite;
        document.body.onmousemove = onuser_activite;
        timeoutHnd = setTimeout('OnTimeoutReached();', logouTimeInterval);
    }

$(document).ready(function () {
    const id = sessionStorage.getItem("Id");
    if(id !==null || id !==undefined)
        sessionStart();
})

//로그아웃
function logout() {
    clearTimeout(timeoutHnd);
    sessionStorage.removeItem("Id");
    window.location.replace("/");
}
//파일 확장자 가져오기
function getExtensionOfFilename(filename) {

    var _fileLen = filename.length;

    /** 
     * lastIndexOf('.') 
     * 뒤에서부터 '.'의 위치를 찾기위한 함수
     * 검색 문자의 위치를 반환한다.
     * 파일 이름에 '.'이 포함되는 경우가 있기 때문에 lastIndexOf() 사용
     */
    var _lastDot = filename.lastIndexOf('.');

    // 확장자 명만 추출한 후 소문자로 변경
    var _fileExt = filename.substring(_lastDot, _fileLen).toLowerCase();

    return _fileExt;
}





