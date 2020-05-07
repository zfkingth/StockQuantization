import React from 'react';
import DataGrid, { Column, } from 'devextreme-react/data-grid';


import { connectTo } from '../../utils/generic';
import { formatNumber } from '../../utils/api';



const getXueQiuUrl = wangyiNum => {
  const pre = "https://xueqiu.com/S/";
  let jys;
  //首字母0表示上海
  if (wangyiNum[0] === '0') {
    jys = 'SH';
  } else {
    jys = 'SZ';
  }
  return pre + jys + wangyiNum.substr(1);
}


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



  getZhangDieFuCellValue(data) {
    return formatNumber(data.zhangDieFu, 2) + '%';
  }



  getHuanShouLvCellValue(data) {
    return formatNumber(data.huanShouLiu, 2) + '%';
  }

  getLiuTongShiZhiCellValue(data) {
    const fv = data.liuTongShiZhi * 10 ** -8;
    return formatNumber(fv, 2);
  }
  getZongShiZhiCellValue(data) {
    const fv = data.zongShiZhi * 10 ** -8;
    return formatNumber(fv, 2);
  }




  cellRender(data) {
    const stockId = data.data.stockId;
    return <a href={getXueQiuUrl(stockId)} target="_blank" rel="noopener noreferrer" >
      {stockId.substr(1)}</a>
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
          <Column caption="股票代码" cellRender={this.cellRender}
            calculateSortValue="stockId"
            allowSorting='true'
          />
          <Column dataField="stockName" caption="股票名称"
          />
          <Column calculateCellValue={this.getZhangDieFuCellValue}
            calculateSortValue="zhangDieFu"
            allowSorting='true'
            alignment="right"
            caption="涨跌幅" />
          <Column dataField="close" caption="价格" />
          <Column calculateCellValue={this.getHuanShouLvCellValue}
            calculateSortValue="huanShouLiu"
            allowSorting='true'
            alignment="right" caption="换手率" />
          <Column calculateCellValue={this.getLiuTongShiZhiCellValue}
            calculateSortValue="liuTongShiZhi"
            allowSorting='true'
            alignment="right"
            caption="流通市值(亿元)" />
          <Column calculateCellValue={this.getZongShiZhiCellValue}
            calculateSortValue="zongShiZhi"
            allowSorting='true'
            alignment="right"
            caption="总市值(亿元)" />

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