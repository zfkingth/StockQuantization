
import _ from 'lodash'
export const shicha =0;// 8 * 3600 * 1000;




export const calcCCI = (historyData, n) => {

  const getTp = (historyData, index) => {
    let price = historyData[index];
    let tp = (price.high + price.low + price.close) / 3;
    return tp;

  }

  //tp 是中价
  let maForTp = [], cci = [], tpSum = 0;
  let Num = n;

  for (let i = 0; i < historyData.length; i += 1) {
    let price = historyData[i];
    let currentDate = new Date(price.date).getTime() + shicha;


    if (i < Num) {
      tpSum += getTp(historyData, i)
      maForTp.push([currentDate, null]);
      cci.push([currentDate, null]);
    } else {
      tpSum += getTp(historyData, i) - getTp(historyData, i - Num);
      let average = tpSum / Num;
      maForTp.push([currentDate, average]);


//     以日CCI计算为例，其计算方法有两种。

// 　　第一种计算过程如下：

// 　　CCI（N日）=（TP－MA）÷MD÷0.015

// 　　其中，TP=（最高价+最低价+收盘价）÷3

// 　　MA=最近N日收盘价的累计之和÷N

// 　　MD=最近N日（MA－收盘价）的累计之和÷N

// 　　0.015为计算系数，N为计算周期

// 　　第二种计算方法表述为中价与中价的N日内移动平均的差除以N日内中价的平均绝对偏差

// 　　其中，中价等于最高价、最低价和收盘价之和除以3

// 　　平均绝对偏差为统计函数

      //calc cci
      let TP = getTp(historyData, i);
      //绝对差
      let jdc = 0;
      for (let j = 0; j < Num; j++) {
        jdc += Math.abs(getTp(historyData, i - j) - average);
      }

      //平均绝对误差
      let mad = jdc / Num;


      let currentCci = (TP - average) / mad / 0.015;
      cci.push([currentDate, _.round(currentCci, 2)]);


    }


  }

  return cci;


}
