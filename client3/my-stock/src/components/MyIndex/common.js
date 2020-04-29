
import _ from 'lodash'





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
      let currentDate = new Date(price.date).getTime() ;
  
  
      if (i < Num) {
        tpSum += getTp(historyData, i)
        maForTp.push([currentDate, null]);
        cci.push([currentDate, null]);
      } else {
        tpSum += getTp(historyData, i) - getTp(historyData, i - Num);
        let average = tpSum / Num;
        maForTp.push([currentDate, average]);
  
  
  
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
  