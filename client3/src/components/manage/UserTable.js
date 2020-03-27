
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

const columns = [
  { name: 'email', title: 'email' },
  { name: 'username', title: 'username' },
  { name: 'roleName', title: 'roleName' },
  { name: 'expiredDate', title: 'expiredDate' },
];


const dateColumns = ['expiredDate'];

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



class UserTable extends React.PureComponent {


  render() {
    const { rows, } = this.props;

    return (
      <div>
        <Grid
          rows={rows}
          columns={columns}
        >
      
          <DateTypeProvider
            for={dateColumns}
          />
         
          <SortingState
          />
          <IntegratedSorting />
          <Table />
          <TableHeaderRow showSortingControls />
        </Grid>
      </div>
    );
  }
}

export default connectTo(
  state => ({
    rows: state.manage.userList,
  }),
  {},
  UserTable
);


