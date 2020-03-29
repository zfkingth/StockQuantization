import React from 'react';
import { render } from 'react-dom';
import Highcharts from 'highcharts/highstock';
import HighchartsReact from 'highcharts-react-official';

const options = {
  yAxis: {
    type: 'logarithmic',
  },
  title: {
    text: 'My stock chart'
  },
  series: [
    {
      data: [1, 2, 1, 4, 3, 6,10,20,30,60,100,150,300]
    }
  ]
};

function App() {
  return (
    <div className="App">
      <HighchartsReact
      highcharts={Highcharts}
      constructorType={'stockChart'}
      options={options}
    />
    </div>
  );
}

export default App;
