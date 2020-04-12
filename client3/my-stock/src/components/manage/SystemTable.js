
import { connectTo } from '../../utils/generic';





import * as React from 'react';
import {
    SortingState, IntegratedSorting, DataTypeProvider
} from '@devexpress/dx-react-grid';
import {
    Grid,
    Table, TableHeaderRow,

} from '@devexpress/dx-react-grid-material-ui';
import Paper from '@material-ui/core/Paper';

const DateFormatter = ({ value }) => {
    if(!value)return 'null';
    const date = new Date(value);
    return date.Format("yyyy-MM-dd hh:mm:ss");

}


const DateTypeProvider = props => (
    <DataTypeProvider
        formatterComponent={DateFormatter}
        {...props}
    />
)


const columns = [
    { name: 'eventName', title: '事件名称' },
    { name: 'lastAriseStartDate', title: '起始时间' },
    { name: 'lastAriseEndDate', title: '结束时间' },
    { name: 'status', title: '状态' },
];
const tableColumnExtensions = [
    { columnName: 'eventName', width: 180 },
    { columnName: 'lastAriseStartDate', width: 180 },
    { columnName: 'lastAriseEndDate', width: 180 },
    { columnName: 'status', width: 80 },
];
const dateColumns = ['lastAriseStartDate', 'lastAriseEndDate'];





class DemoBase extends React.PureComponent {


    render() {
        const { rows, } = this.props;

        return (
            <Paper>
                <Grid rows={rows} columns={columns}      >

                    <DateTypeProvider
                        for={dateColumns}
                    />

                    <SortingState
                        defaultSorting={[{ columnName: 'lastAriseEndDate', direction: 'desc' }]}
                    />
                    <IntegratedSorting />
                    <Table columnExtensions={tableColumnExtensions} />
                    <TableHeaderRow showSortingControls />
                </Grid>
            </Paper>
        );
    }
}


export default connectTo(
    state => ({
        rows: state.manage.systemStatus.statusTable,
    }),
    {},
    DemoBase
);
