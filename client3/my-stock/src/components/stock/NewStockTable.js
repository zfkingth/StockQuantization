import React from 'react';
import DataGrid, { Column, Editing } from 'devextreme-react/data-grid';


import { connectTo } from '../../utils/generic';

class DemoBase extends React.PureComponent {

  constructor(props) {
    super(props);
    this.state = { events: [] };


  }

  logEvent = (eventName) => {
    this.setState((state) => {
      return { events: [eventName].concat(state.events) };
    });
  }

  clearEvents = () => {
    this.setState({ events: [] });
  }



  render() {

    const { rows, } = this.props;
    return (
      <React.Fragment>
        <DataGrid
          id="gridContainer"
          dataSource={rows}
          keyExpr="stockId"
          showBorders={false}
        >

          <Column dataField="date" caption="日期" dataType="date" format="yyyy-MM-dd" />
          <Column dataField="stockId" caption="股票代码" />
          <Column dataField="stockName" caption="股票名称" />
          <Column dataField="zhangDieFu" caption="涨跌幅" />
          <Column dataField="close" caption="价格" />
          <Column dataField="huanShouLiu" caption="换手率" />
          <Column dataField="liuTongShiZhi" caption="流通市值(亿元)" />
          <Column dataField="zongShiZhi" caption="总市值(亿元)" />

        </DataGrid>

      </React.Fragment>
    );
  }
}

export default connectTo(
  state => ({
    rows: state.stock.stockList,
  }),
  {},
  DemoBase
);