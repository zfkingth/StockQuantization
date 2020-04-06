
import React from "react";

import styled from 'styled-components'

import { get, fetchData } from '../../utils/api'
import * as URL from '../../constants/api'
import CircularProgress from '@material-ui/core/CircularProgress';

import { transformError } from '../../utils/api'
import _ from 'lodash'
import Highcharts from 'highcharts/highstock';
import HighchartsReact from 'highcharts-react-official';


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


const shicha = 8 * 3600 * 1000;


function handleMarginData(marginArray) {
  let myMap = new Map();
  for (let i = 0; i < marginArray.length; i++) {
    let item = marginArray[i];
    let currentDate = new Date(item.date).getTime() + shicha;
    let val = myMap.get(currentDate);
    if (!val) {
      val = 0;
    }
    val += item.finValue;
    myMap.set(currentDate, val);
  }
  let marginForChart = [];

  let preVal = undefined;
  for (const [key, val] of myMap) {
    if (preVal !== undefined) {
      let change = val - preVal;

      marginForChart.push([
        key,
        _.round(change / 10 ** 8, 2)
      ]);


    }
    preVal = val;
  }


  return marginForChart;
}


const maset = [5, 20, 60];
const prepareHistoryData = (historyData) => {


  let ma = [], ohlc = [], money = [];
  for (let i = 0; i < historyData.length; i += 1) {
    let price = historyData[i];
    let currentDate = new Date(price.date).getTime() + shicha;
    ohlc.push({
      x: currentDate,

      open: price.open,
      high: price.high,
      low: price.low,
      close: price.close,
      preclose: price.preclose,

    });
    money.push([
      currentDate,
      _.round(price.money / 10 ** 8, 2)
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


const createOption = (stockInfo, historyData, marginData) => {



  let marginForChart = handleMarginData(marginData);

  let rt = prepareHistoryData(historyData);
  let ohlc = rt.ohlc, money = rt.money, ma = rt.ma;
  // set the allowed units for data grouping



  let stockOptions = {

    chart: {
      // height: (9 / 16 * 100) + '%' // 16:9 ratio
      height: (9 / 16 * 100) + '%'  // 16:9 ratio
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
          return s + '<br/>' + point.series.name + ': ' +
            point.y;
        }, '<b>' + this.x + '</b>');
      },
    },

    yAxis: [{
      type: 'logarithmic',
      labels: {
        align: 'right',
        x: -3
      },
      title: {
        text: '股价'
      },
      height: '80%',
      resize: {
        enabled: true
      },
      lineWidth: 2
    }, {
      labels: {
        align: 'right',
        x: -3
      },
      title: {
        text: '成交额'
      },
      top: '80%',
      height: '10%',
      offset: 0,
      lineWidth: 2
    }, {
      labels: {
        align: 'right',
        x: -3
      },
      title: {
        text: '融资'
      },
      top: '90%',
      height: '10%',
      offset: 0,
      lineWidth: 2
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
      color: "blue",
      yAxis: 2

    },

    ]


  }

  return stockOptions;
}


export default class tempcontrol extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      received: undefined,
      error: undefined,
      options: undefined
    };
  }

  componentDidMount() {
    this.getDataAsync('399006.XSHE');


  }


  getDataAsync = async function (stockId) {
    try {
      //不能更改这个顺序，是通过索引使用
      const p0 = fetchData(get, URL.GETSTOCK(stockId));
      const p1 = fetchData(get, URL.GETVALUES(stockId));
      const p2 = fetchData(get, URL.GETMARGIN);

      const result = await Promise.all([p0, p1, p2]);

      let opt = createOption(result[0], result[1], result[2]);


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
