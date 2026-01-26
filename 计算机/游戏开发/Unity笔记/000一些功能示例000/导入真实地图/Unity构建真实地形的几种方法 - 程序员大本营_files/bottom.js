function validate(){var a=document.getElementById("s").value;if(a.replace(/^\s+(.*?)\s+$/,'$1')==''){alert('搜索关键词不能为空。');return false;}}
function pagedSearch(index){document.getElementById("curPage").value=index;document.getElementById("searchForm").submit();}
document.writeln("<script type=\"text/javascript\" src=\"//js.users.51.la/20761015.js\"></script>");