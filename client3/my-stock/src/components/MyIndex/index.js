
import React from "react";

import styled from 'styled-components'

import { get, fetchData } from '../../utils/api'
import * as URL from '../../constants/api'
import CircularProgress from '@material-ui/core/CircularProgress';

import { transformError } from '../../utils/api'
import _ from 'lodash'
import Highcharts from 'highcharts/highstock';
import HighchartsReact from 'highcharts-react-official';

import { calcCCI } from './common'


export const maset = [5, 20, 60];
const MulRowsWrap = styled.div`
 
  flex-direction:column;
  display: flex;
  justify-content: flex-start;
  align-items: stretch;
`
const Loading = styled.div`
 
  display: flex;
  justify-content: center;
  align-items: center;
`
const StyleError = styled.p`
 
color:red;
`



function timeStamp2String(time) {
  const datetime = new Date();
  datetime.setTime(time);
  const year = datetime.getFullYear();
  const month = datetime.getMonth() + 1 < 10 ? "0" + (datetime.getMonth() + 1) : datetime.getMonth() + 1;
  const date = datetime.getDate() < 10 ? "0" + datetime.getDate() : datetime.getDate();
  // var hour = datetime.getHours()< 10 ? "0" + datetime.getHours() : datetime.getHours();  
  // var minute = datetime.getMinutes()< 10 ? "0" + datetime.getMinutes() : datetime.getMinutes();  
  // var second = datetime.getSeconds()< 10 ? "0" + datetime.getSeconds() : datetime.getSeconds();  
  return year + "-" + month + "-" + date;  //+" "+hour+":"+minute+":"+second;  
}

function handleMarginData(marginArray) {
  let marginForChart = [];

  let preVal = undefined;
  for (let i = 0; i < marginArray.length; i++) {
    let item = marginArray[i];
    let currentDate = new Date(item.date).getTime();
    let val = item.finValue;

    if (preVal !== undefined) {
      let change = val - preVal;

      marginForChart.push([
        currentDate,
        _.round(change / 10 ** 8, 2)
      ]);


    }
    preVal = val;
  }


  return marginForChart;
}

const prepareMarketDeal = mk => {
  let myMap = new Map();
  for (let i = 0; i < mk.length; i++) {
    let item = mk[i];
    let currentDate = new Date(item.date).getTime();
    let val = myMap.get(currentDate);
    if (!val) {
      val = 0;
    }
    val += item.drzjlr;
    myMap.set(currentDate, val);
  }
  let dataForChart = [];

  for (const [key, val] of myMap) {

    dataForChart.push([
      key,
      _.round(val, 2)
    ]);


  }


  return dataForChart;
}


const prepareHistoryData = (historyData) => {


  let ma = [], ohlc = [], money = [];
  for (let i = 0; i < historyData.length; i += 1) {
    let price = historyData[i];
    let currentDate = new Date(price.date).getTime();
    ohlc.push({
      x: currentDate,

      open: price.open,
      high: price.high,
      low: price.low,
      close: price.close,
      preclose: _.round(price.close / (100 + price.zhangDieFu) * 100, 2)

    });
    money.push([
      currentDate,
      _.round(price.amount / 10 ** 8, 2)
    ]);

    for (let index = 0; index < maset.length; index++) {

      let maNum = maset[index];
      if (typeof ma['ma' + maNum] == "undefined") {
        ma['ma' + maNum] = [];
      }
      if (typeof ma[maNum + 'total'] == "undefined") {
        ma[maNum + 'total'] = 0;
      }
      if (i < maNum) {
        ma[maNum + 'total'] += historyData[i].close;
        ma['ma' + maNum].push([currentDate, null]);
      } else {
        ma[maNum + 'total'] += (historyData[i].close - historyData[i - maNum].close);
        let average = (ma[maNum + 'total'] / maNum);
        ma['ma' + maNum].push([currentDate, _.round(average, 2)]);
      }


    }
  }
  return { ohlc, money, ma };
}


const prepareStaData = (staPrice) => {

  let highData = [], lowData = [], failData = [];

  for (let i = 0; i < staPrice.length; i += 1) {
    let sta = staPrice[i];
    let currentDate = new Date(sta.date).getTime();

    highData.push([currentDate, sta.highlimitNum]);
    lowData.push([currentDate, -sta.lowlimitNum]);
    failData.push([currentDate, -sta.failNum]);

  }

  return { highData, lowData, failData }

}

const createOption = (stockInfo, historyData, marginData, marketDeal, staPrice) => {



  let rt = prepareHistoryData(historyData);
  let ohlc = rt.ohlc, money = rt.money, ma = rt.ma;
  // set the allowed units for data grouping

  let cciData = calcCCI(historyData, 14);


  const marginForChart = handleMarginData(marginData);
  const marketForChart = prepareMarketDeal(marketDeal);

  const staForChart = prepareStaData(staPrice);


  let stockOptions = {

    chart: {
      // height: (9 / 16 * 100) + '%' // 16:9 ratio
      height: (9 / 16 * 100) * 1.2 + '%'  // 16:9 ratio
    },
    rangeSelector: {
      selected: 1,
      inputDateFormat: '%Y-%m-%d'
    },
    title: {
      text: stockInfo.displayname
    },
    xAxis: {
      dateTimeLabelFormats: {
        millisecond: '%H:%M:%S.%L',
        second: '%H:%M:%S',
        minute: '%H:%M',
        hour: '%H:%M',
        day: '%m-%d',
        week: '%m-%d',
        month: '%y-%m',
        year: '%Y'
      }
    },
    tooltip: {
      shared: true,
      crosshairs: true,
      split: false,
      // 时间格式化字符
      dateTimeLabelFormats: {
        day: '%Y-%m-%d'
      },

      formatter: function () {
        return this.points.reduce(function (s, point) {

          let con = point.series.name + ': ' + point.y;
          if (point.series.type === 'candlestick') {
            const ppt = point.point;//.point.options;
            const zhangdiefu = _.round((ppt.close - ppt.preclose) / ppt.preclose * 100, 2);
            con = ''
              + '涨幅：' + zhangdiefu + '%<br/>'
              + '开盘：' + ppt.open + '<br/>'
              + "最低：" + ppt.low + '<br/>'
              + "最高：" + ppt.high + '<br/>'
              + "收盘：" + ppt.close + '<br/>'
              ;
          }

          const rt = s + '<br/>' + con;

          return rt;
        }, '<b>' + timeStamp2String(this.x) + '</b>');
      },
    },

    yAxis: [
      {
        type: 'logarithmic',
        labels: {
          align: 'right',
          x: -3
        },
        title: {
          text: '股价'
        },
        height: '40%',
        resize: {
          enabled: true
        },
        lineWidth: 2
      },
      {
        labels: {
          align: 'right',
          x: -3
        },
        title: {
          text: '成交额'
        },
        top: '40%',
        height: '5%',
        offset: 0,
        lineWidth: 2
      },
      {
        labels: {
          align: 'right',
          x: -3
        },
        title: {
          text: '融资'
        },
        top: '45%',
        height: '15%',
        offset: 0,
        lineWidth: 2
      },
      {
        labels: {
          align: 'right',
          x: -3
        },
        title: {
          text: '陆股通流入'
        },
        top: '60%',
        height: '15%',
        offset: 0,
        lineWidth: 2,

      },
      {
        labels: {
          align: 'right',
          x: -3
        },
        title: {
          text: 'CCI'
        },
        top: '75%',
        height: '10%',
        offset: 0,
        lineWidth: 2,

      },
      {
        labels: {
          align: 'right',
          x: -3
        },
        title: {
          text: '涨跌停'
        },
        top: '85%',
        height: '15%',
        offset: 0,
        lineWidth: 2,

      }
    ],
    series: [{
      type: 'candlestick',
      turboThreshold: 10000,
      name: stockInfo.displayname,
      color: 'green',
      lineColor: 'green',
      upColor: 'red',
      upLineColor: 'red',

      navigatorOptions: {
        color: Highcharts.getOptions().colors[0]
      },
      data: ohlc,


    }, {
      type: 'line',
      name: '5日线',
      data: ma['ma5'],
      color: "green",
      yAxis: 0

    },
    {
      type: 'line',
      name: '20日线',
      data: ma['ma20'],
      color: "blue",
      yAxis: 0

    },
    {
      type: 'line',
      name: '60日线',
      data: ma['ma60'],
      color: "red",
      yAxis: 0

    },

    {
      type: 'column',
      name: '成交金额',
      data: money,
      yAxis: 1,

    },
    {
      type: 'column',
      name: '融资变化',
      data: marginForChart,
      yAxis: 2,



      color: 'red', // 默认颜色
      zones: [{
        // 小于0显示 'green',大于0的则使用默认颜色 'red'
        value: 0,
        color: 'green',
      }]


    },

    {
      type: 'column',
      name: '陆股通流入',
      data: marketForChart,
      yAxis: 3,

      color: 'red', // 默认颜色
      zones: [{
        // 小于0显示 'green',大于0的则使用默认颜色 'red'
        value: 0,
        color: 'green',
      }]

    },


    {
      type: 'line',
      name: 'CCI',
      data: cciData,
      yAxis: 4,

      color: 'brown', // 默认颜色

      zones: [
        {
          value: -100,
          color: 'green',
        },
        {
          value: 100,
          color: '#7cb5ec',
          dashStyle: 'dot'
        },
        {
          color: 'red'
        }
      ]

    },


    {
      type: 'column',
      name: '涨停个数',
      data: staForChart.highData,
      yAxis: 5,

      color: 'red', // 默认颜色

    },


    {
      type: 'column',
      name: '跌停个数',
      data: staForChart.lowData,
      yAxis: 5,

      color: 'green', // 默认颜色

    },


      // {
      //   type: 'column',
      //   name: '炸板个数',
      //   data: staForChart.failData,
      //   yAxis: 5,

      //   color: 'blue', // 默认颜色

      // },

    ]


  }

  return stockOptions;
}




class Basecontrol extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      received: undefined,
      error: undefined,
      options: undefined
    };
  }

  componentDidMount() {
    this.getDataAsync(this.props.stockId);


  }


  getDataAsync = async function (stockId) {
    try {
      //不能更改这个顺序，是通过索引使用
      const p0 = fetchData(get, URL.GETSTOCK(stockId));
      const p1 = fetchData(get, URL.GETVALUES(stockId));
      const p2 = fetchData(get, URL.GETMARGIN);
      const p3 = fetchData(get, URL.GETMARKETDEAL);
      const p4 = fetchData(get, URL.GetStaPrice);

      const allData = await Promise.all([p0, p1, p2, p3, p4]);



      let opt = createOption(...allData);


      this.setState({ received: true, options: opt });
    } catch (err) {

      console.log("received data error : " + err);
      this.setState({ error: transformError(err) });
    }

  }



  render() {
    return this.state.error ?
      (
        <StyleError>错误：{this.state.error}</StyleError>
      ) :
      this.state.received ? (

        <MulRowsWrap>
          <HighchartsReact
            highcharts={Highcharts}
            constructorType={'stockChart'}
            options={this.state.options}
          />
        </MulRowsWrap>
      ) :
        (
          <Loading>
            <CircularProgress />
          </Loading>
        )

  }

}




const IndexCyb = (props) => {
  return <Basecontrol stockId="399006.XSHE" />;
}

const IndexHS300 = (props) => {
  return <Basecontrol stockId="000300.XSHG" />;
}



const IndexSZCZ = (props) => {
  return <Basecontrol stockId="1399001" />;
}

const IndexSZZZ = (props) => {
  return <Basecontrol stockId="399106.XSHE" />;
}

export { IndexCyb, IndexHS300, IndexSZCZ, IndexSZZZ };