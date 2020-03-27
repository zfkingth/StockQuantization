
class RequestError {
  constructor(status, message) {
    this.status = status
    this.message = message
  }
}

export const headers = () => ({
  'Content-Type': 'application/json',
  Authorization: `Bearer ${localStorage.getItem('token')}`
})

export const makePostOptions = data => ({
  method: 'POST',
  mode: 'cors',
  headers: headers(),
  body: JSON.stringify(data)
})

export const makePatchOptions = data => ({
  ...makePostOptions(data),
  method: 'PATCH'
})

export const getOptions = () => ({
  method: 'GET',
  headers: headers()
})

export const deleteOptions = () => ({
  method: 'DELETE',
  mode: 'cors',
  headers: headers()
})





const request = async (url, options) => {
  let timeout;
  const response = await Promise.race([
    fetch(url, options),
    new Promise(function (resolve, reject) {
      timeout = setTimeout(() => {
        reject(new Error("请求超时"))
      },
        7000);
    })
  ])
    .finally(() => {
      console.log('request promise finally is executed.');
      clearTimeout(timeout);
    });

  const { status } = response;

  console.log('request promise status is ' + status);
  if (status === 204) return {};


  if (status >= 200 && status < 400) {
    const json = response.json();
    console.log('request promise json data is ');
    return json;
  }

  // const message=await response.json();
  //这里有可能返回html 的详细错误页面。

  //没有网络或者超时时会使用reject，代码不会执行到这里。
  //有网络，但是出错时会有状态码，也就是status 
  const message = await response.text();
  console.log('request promise throw RequestError : ' + message);

  throw new RequestError(status, message);

}






export const plainGet = url =>
  request(url, {
    method: 'GET',
    header: { 'Content-Type': 'application/json' }
  })
export const plainPost = (url, data) =>
  request(url, {
    method: 'POST',
    body: JSON.stringify(data)
  })
export const get = url => request(url, getOptions())
export const post = (url, data) => request(url, makePostOptions(data))
export const patch = (url, data) => request(url, makePatchOptions(data))
export const del = (url, id) => request(url + id, deleteOptions())

// mys custom 
//args第一個參數是方法，第二個參數是方法对应的参数
export const fetchData = async (...args) => {
  try {
    const data = await args[0](...args.slice(1));
    console.log('callWith401_403Handle data: ' + JSON.stringify(data));
    return data;
  } catch (ex) {

    const status = ex.status;
    if (status === 401) {
      ex.message = '未授权 由于凭据无效 访问被拒绝。';
    } else if (status === 403) {
      ex.message = '禁止访问 访问被拒绝。您无权使用所提供的凭据查看此目录或页面。';
    }
    console.log('callWith401_403Handle catch clause ' + JSON.stringify(ex));
    throw ex;
  }
}




// eslint-disable-next-line no-extend-native
Date.prototype.Format = function (fmt) {
  var o = {
    "y+": this.getFullYear(),
    "M+": this.getMonth() + 1,                 //月份
    "d+": this.getDate(),                    //日
    "h+": this.getHours(),                   //小时
    "m+": this.getMinutes(),                 //分
    "s+": this.getSeconds(),                 //秒
    "q+": Math.floor((this.getMonth() + 3) / 3), //季度
    "S+": this.getMilliseconds()             //毫秒
  };
  for (var k in o) {
    if (new RegExp("(" + k + ")").test(fmt)) {
      if (k === "y+") {
        fmt = fmt.replace(RegExp.$1, ("" + o[k]).substr(4 - RegExp.$1.length));
      }
      else if (k === "S+") {
        var lens = RegExp.$1.length;
        lens = lens === 1 ? 3 : lens;
        fmt = fmt.replace(RegExp.$1, ("00" + o[k]).substr(("" + o[k]).length - 1, lens));
      }
      else {
        fmt = fmt.replace(RegExp.$1, (RegExp.$1.length === 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
      }
    }
  }
  return fmt;
};

function formatNumber(num, precision, separator) {
  var parts;
  // 判断是否为数字
  if (!isNaN(parseFloat(num)) && isFinite(num)) {
    // 把类似 .5, 5. 之类的数据转化成0.5, 5, 为数据精度处理做准, 至于为什么
    // 不在判断中直接写 if (!isNaN(num = parseFloat(num)) && isFinite(num))
    // 是因为parseFloat有一个奇怪的精度问题, 比如 parseFloat(12312312.1234567119)
    // 的值变成了 12312312.123456713
    num = Number(num);
    // 处理小数点位数
    num = (typeof precision !== 'undefined' ? num.toFixed(precision) : num).toString();
    // 分离数字的小数部分和整数部分
    parts = num.split('.');
    // 整数部分加[separator]分隔, 借用一个著名的正则表达式
    parts[0] = parts[0].toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1' + (separator || ','));

    return parts.join('.');
  }
  return NaN;
};

export { formatNumber }


export const transformError = (errorInfo) => {
  let ret = '';
  if (errorInfo.status) {
    ret += 'status:' + errorInfo.status;
  }

  if (errorInfo.message) {
    ret += ' ' + errorInfo.message;
  }

  if (ret.length === 0) {
    ret = errorInfo.toString();
  }

  return ret;

}
