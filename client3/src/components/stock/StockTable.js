
import * as React from 'react';
import {
  SortingState,
  IntegratedSorting,
  DataTypeProvider,
} from '@devexpress/dx-react-grid';
import {
  Grid,
  Table,
  TableHeaderRow,
} from '@devexpress/dx-react-grid-material-ui';
import { connectTo } from '../../utils/generic';
import { formatNumber } from '../../utils/api';

const columns = [
  { name: 'date', title: '日期' },
  { name: 'stockId', title: '股票代码' },
  { name: 'stockName', title: '股票名称' },
  { name: 'zhangDieFu', title: '涨跌幅' },
  { name: 'close', title: '价格' },
  {
    name: 'huanShouLiu', title: '换手率',
  },
  { name: 'liuTongShiZhi', title: '流通市值(亿元)' },
  { name: 'zongShiZhi', title: '总市值(亿元)' },
];

const tableColumnExtensions = [
  { columnName: 'zhangDieFu', align: 'right' },
  { columnName: 'close', align: 'right' },
  { columnName: 'huanShouLiu', align: 'right' },
  { columnName: 'zhangDieFu', align: 'right' },
  { columnName: 'liuTongShiZhi', align: 'right' },
  { columnName: 'zongShiZhi', align: 'right' },
];

const dateColumns = ['date'];
const idColumns = ['stockId'];
const percentColumns = ['zhangDieFu', 'huanShouLiu'];
const shizhiColumns = ['liuTongShiZhi', 'zongShiZhi'];

const DateFormatter = ({ value }) => {
  const date = new Date(value);
  return date.Format("yyyy-MM-dd");

}


const DateTypeProvider = props => (
  <DataTypeProvider
    formatterComponent={DateFormatter}
    {...props}
  />
);

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

const IdFormatter = ({ value }) => (

  <a href={getXueQiuUrl(value)} target="_blank" rel="noopener noreferrer" >
    {value.substr(1)}</a>



);
const IdTypeProvider = props => (
  <DataTypeProvider
    formatterComponent={IdFormatter}
    {...props}
  />
);


const PercentFormatter = ({ value }) => {
  const fv = (value);
  return formatNumber(fv, 2) + '%';

};
const PercentTypeProvider = props => (
  <DataTypeProvider
    formatterComponent={PercentFormatter}
    {...props}
  />
);

const ShizhiFormatter = ({ value }) => {
  const fv = value * 10 ** -8;
  return formatNumber(fv, 2);

};
const ShizhiTypeProvider = props => (
  <DataTypeProvider
    formatterComponent={ShizhiFormatter}
    {...props}
  />
);



class StockTable extends React.PureComponent {


  render() {
    const { rows, } = this.props;

    return (
      <div>
        <Grid
          rows={rows}
          columns={columns}
        >
          <IdTypeProvider
            for={idColumns}
          />
          <DateTypeProvider
            for={dateColumns}
          />
          <PercentTypeProvider
            for={percentColumns}
          />
          <ShizhiTypeProvider
            for={shizhiColumns}
          />
          <SortingState
            defaultSorting={[{ columnName: 'zhangDieFu', direction: 'desc' }]}
          />
          <IntegratedSorting />
          <Table columnExtensions={tableColumnExtensions} />
          <TableHeaderRow showSortingControls />
        </Grid>
      </div>
    );
  }
}

export default connectTo(
  state => ({
    rows: state.stock.stockList,
  }),
  {},
  StockTable
);


