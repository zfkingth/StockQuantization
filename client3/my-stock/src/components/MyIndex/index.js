
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



const createOption = stockData => {
 
  let data = stockData;
  const maset = [5, 20, 60];
  let ma = [];

  let ohlc = [],
    volume = [],
    dataLength = data.length;
  // set the allowed units for data grouping

  for (let i = 0; i < dataLength; i += 1) {
    let price = data[i];
    let currentDate=new Date(price.date).getTime()+8*3600*1000;
    ohlc.push([
      currentDate,
      price.open,
      price.high,
      price.low,
      price.close,

    ]);
    volume.push([
      currentDate,
      price.volume

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
        ma[maNum + 'total'] += data[i].close;
        ma['ma' + maNum].push([currentDate, null]);
      } else {
        ma[maNum + 'total'] += (data[i].close - data[i - maNum].close);
        let average = (ma[maNum + 'total'] / maNum);
        ma['ma' + maNum].push([currentDate, _.round(average, 2)]);
      }


    }
  }


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
      text: '创业板指数'
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
      }
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
      height: '90%',
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
        text: '成交量'
      },
      top: '90%',
      height: '10%',
      offset: 0,
      lineWidth: 2
    }],
    series: [{
      type: 'candlestick',
      name: data[0].code,
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
      name: '成交量',
      data: volume,
      yAxis: 1,

    }]


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
      const data = await fetchData(get, URL.GETVALUES(stockId));

      let opt = createOption(data);


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

