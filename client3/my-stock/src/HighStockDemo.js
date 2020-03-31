

import React from 'react';
import Highcharts from 'highcharts/highstock';
import HighchartsReact from 'highcharts-react-official';

import pinganData from './pinganData'

// // Load Highcharts modules
// require('highcharts/indicators/indicators')(Highcharts)
// require('highcharts/indicators/pivot-points')(Highcharts)
// require('highcharts/indicators/macd')(Highcharts)
// require('highcharts/modules/exporting')(Highcharts)
// require('highcharts/modules/map')(Highcharts)



const createOption = stockData => {
    let data = stockData.data;
    const maset = [5, 20, 60];
    let ma = [];

    let ohlc = [],
        volume = [],
        dataLength = data.length,
        // set the allowed units for data grouping
        groupingUnits = [[
            'week',                         // unit name
            [1]                             // allowed multiples
        ], [
            'month',
            [1, 2, 3, 4, 6]
        ]];

    for (let i = 0; i < dataLength; i += 1) {
        ohlc.push([
            data[i][0], // the date
            data[i][1], // open
            data[i][2], // high
            data[i][3], // low
            data[i][4] // close
        ]);
        volume.push([
            data[i][0], // the date
            data[i][5] // the volume
        ]);

        for (let index = 0; index < maset.length; index++) {

            let value = maset[index];
            if (typeof ma['ma' + value] == "undefined") {
                ma['ma' + value] = [];
            }
            if (typeof ma[value + 'total'] == "undefined") {
                ma[value + 'total'] = 0;
            }
            if (i < value) {
                ma[value + 'total'] += data[i][4];
                ma['ma' + value].push([data[i][0], null]);
            } else {
                ma[value + 'total'] += (data[i][4] - data[i - value][4]);
                ma['ma' + value].push([data[i][0], ma[value + 'total'] / value]);
            }


        }
    }


    let stockOptions = {

        chart: {
            // height: (9 / 16 * 100) + '%' // 16:9 ratio
            height: 900 // 16:9 ratio
        },


        rangeSelector: {
            selected: 1,
            inputDateFormat: '%Y-%m-%d'
        },
        title: {
            text: '平安银行历史股价'
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
            split: false,
            shared: true,
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
            name: '平安银行',
            color: 'green',
            lineColor: 'green',
            upColor: 'red',
            upLineColor: 'red',
            tooltip: {
            },
            navigatorOptions: {
                color: Highcharts.getOptions().colors[0]
            },
            data: ohlc,
            dataGrouping: {
                units: groupingUnits
            },
            id: 'sz'
        }, {
            type: 'line',
            name: 'ma5',
            data: ma['ma5'],
            color: "green",
            yAxis: 0

        },

        {
            type: 'column',
            data: volume,
            yAxis: 1,
            dataGrouping: {
                units: groupingUnits
            }
        }]


    }

    return stockOptions;
}



class HighStockDemo extends React.PureComponent {
    render() {
        return (
            <HighchartsReact
                highcharts={Highcharts}
                constructorType={'stockChart'}
                options={createOption(pinganData)}
            />
        );
    }
}
export default HighStockDemo;
