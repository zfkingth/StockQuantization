import { updateUser, deleteUser } from '../../actions/auth';
import { goToErrorPage, } from '../../actions/navigation'

import { connectTo } from '../../utils/generic';





import * as React from 'react';
import {
    SortingState, EditingState, PagingState,
    IntegratedPaging, IntegratedSorting,
} from '@devexpress/dx-react-grid';
import {
    Grid,
    Table, TableHeaderRow, TableEditRow, TableEditColumn,
    PagingPanel, DragDropProvider, TableColumnReordering,
    TableFixedColumns,
} from '@devexpress/dx-react-grid-material-ui';
import Paper from '@material-ui/core/Paper';
import Button from '@material-ui/core/Button';
import Input from '@material-ui/core/Input';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import TableCell from '@material-ui/core/TableCell';

import { withStyles } from '@material-ui/core/styles';



const styles = theme => ({
    lookupEditCell: {
        paddingTop: theme.spacing.unit * 0.875,
        paddingRight: theme.spacing.unit,
        paddingLeft: theme.spacing.unit,
    },
    dialog: {
        width: 'calc(100% - 16px)',
    },
    inputRoot: {
        width: '100%',
    },
});

const AddButton = ({ onExecute }) => (
    <div style={{ textAlign: 'center' }}>
        <Button
            color="primary"
            onClick={onExecute}
            title="Create new row"
        >
            New
    </Button>
    </div>
);

const EditButton = ({ onExecute }) => (
    <Button onClick={onExecute} title="Edit row">Edit
    </Button>
);

const DeleteButton = ({ onExecute }) => (
    <Button
        onClick={() => {
            // eslint-disable-next-line
            if (window.confirm('Are you sure you want to delete this row?')) {
                onExecute();
            }
        }}
        title="Delete row"
    >
       Delete
    </Button>
);

const CommitButton = ({ onExecute }) => (
    <Button onClick={onExecute} title="Save changes">
      Save
    </Button>
);

const CancelButton = ({ onExecute }) => (
    <Button color="secondary" onClick={onExecute} title="Cancel changes">
       Cancel
    </Button>
);

const commandComponents = {
    add: AddButton,
    edit: EditButton,
    delete: DeleteButton,
    commit: CommitButton,
    cancel: CancelButton,
};

const Command = ({ id, onExecute }) => {
    const CommandButton = commandComponents[id];
    return (
        <CommandButton
            onExecute={onExecute}
        />
    );
};



const LookupEditCellBase = ({
    availableColumnValues, value, onValueChange, classes,
}) => (
        <TableCell
            className={classes.lookupEditCell}
        >
            <Select
                value={value}
                onChange={event => onValueChange(event.target.value)}
                input={(
                    <Input
                        classes={{ root: classes.inputRoot }}
                    />
                )}
            >
                {availableColumnValues.map(item => (
                    <MenuItem key={item} value={item}>
                        {item}
                    </MenuItem>
                ))}
            </Select>
        </TableCell>
    );
export const LookupEditCell = withStyles(styles, { name: 'ControlledModeDemo' })(LookupEditCellBase);

const Cell = (props) => {
  
    return <Table.Cell {...props} />;
};

const EditCell = (props) => {

    return <TableEditRow.Cell {...props} />;
};

const getRowId = row => row.tempid;

class DemoBase extends React.PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            columns: [
                { name: 'email', title: 'Email' },
                { name: 'username', title: 'Username' },
                { name: 'roleName', title: 'RoleName' },
                { name: 'expiredDate', title: 'ExpiredDate' },
            ],
            tableColumnExtensions: [
                { columnName: 'email', width: 180 },
                { columnName: 'username', width: 100 },
                { columnName: 'roleName', width: 80, align: 'right' },
                { columnName: 'expiredDate', width: 180 },
            ],

            editingStateColumnExtensions: [
                { columnName: 'email', editingEnabled: false },
                { columnName: 'username', editingEnabled: false },
            ],
            rows:
                this.props.stateRows.map((item, index) => { return { tempid: index, ...item } })
            ,
            sorting: [],
            editingRowIds: [],
            addedRows: [],
            rowChanges: {},
            currentPage: 0,
            pageSize: 0,
            pageSizes: [5, 10, 0],
            columnOrder: ['email', 'username', 'roleName', 'expiredDate',],
            currencyColumns: ['amount'],
            percentColumns: ['discount'],
            leftFixedColumns: [TableEditColumn.COLUMN_TYPE],

        };
        const getStateRows = () => {
            const { rows } = this.state;
            return rows;
        };

        this.changeSorting = sorting => this.setState({ sorting });
        this.changeEditingRowIds = editingRowIds => this.setState({ editingRowIds });
        this.changeAddedRows = addedRows => this.setState({
            addedRows: addedRows.map(row => (Object.keys(row).length ? row : {
                email: 'email',
                username: 'aa',
                expiredDate: new Date().toISOString().split('T')[0],
                roleName: 'bb',
            })),
        });
        this.changeRowChanges = rowChanges => this.setState({ rowChanges });
        this.changeCurrentPage = currentPage => this.setState({ currentPage });
        this.changePageSize = pageSize => this.setState({ pageSize });
        this.commitChanges = ({ added, changed, deleted }) => {
            let { rows } = this.state;
            if (added) {
                const startingAddedId = rows.length > 0 ? rows[rows.length - 1].tempid + 1 : 0;
                rows = [
                    ...rows,
                    ...added.map((row, index) => ({
                        tempid: startingAddedId + index,
                        ...row,
                    })),
                ];
            }
            if (changed) {
                rows = rows.map(row => {
                    if (changed[row.tempid]) {
                        const retItem = { ...row, ...changed[row.tempid] };
                        let values = { ...retItem };
                        delete values.tempid;
                        this.props.updateUser({
                            id: values.id,
                            values,
                            reject: (message) => { this.props.goToErrorPage(message) }
                        });
                        return retItem;
                    }
                    else {
                        return row;
                    }
                }
                );
            }
            if (deleted) {
                console.log('deleted is executed');
                const deletedSet = new Set(deleted);
                rows = rows.filter(row => {

                    if (deletedSet.has(row.tempid)) {
                        this.props.deleteUser({
                            id: row.id,
                            reject: (message) => { this.props.goToErrorPage(message) }
                        });
                        return false;
                    } else {
                        return true;
                    }
                });
            }
            this.setState({ rows });
        };
        this.deleteRows = (deletedIds) => {
            const rows = getStateRows().slice();
            deletedIds.forEach((rowId) => {
                const index = rows.findIndex(row => row.tempid === rowId);
                if (index > -1) {
                    rows.splice(index, 1);
                }
            });
            return rows;
        };
        this.changeColumnOrder = (order) => {
            this.setState({ columnOrder: order });
        };
    }

    render() {
        const {
            rows,
            columns,
            tableColumnExtensions,
            editingStateColumnExtensions,
            sorting,
            editingRowIds,
            addedRows,
            rowChanges,
            currentPage,
            pageSize,
            pageSizes,
            columnOrder,
            leftFixedColumns,
        } = this.state;

        return (
            <Paper>
                <Grid
                    rows={rows}
                    columns={columns}
                    getRowId={getRowId}
                >
                    <SortingState
                        sorting={sorting}
                        onSortingChange={this.changeSorting}
                    />
                    <PagingState
                        currentPage={currentPage}
                        onCurrentPageChange={this.changeCurrentPage}
                        pageSize={pageSize}
                        onPageSizeChange={this.changePageSize}
                    />
                    <EditingState
                        editingRowIds={editingRowIds}
                        onEditingRowIdsChange={this.changeEditingRowIds}
                        rowChanges={rowChanges}
                        onRowChangesChange={this.changeRowChanges}
                        addedRows={addedRows}
                        onAddedRowsChange={this.changeAddedRows}
                        onCommitChanges={this.commitChanges}
                        columnExtensions={editingStateColumnExtensions}
                    />
                    <IntegratedSorting />
                    <IntegratedPaging />


                    <DragDropProvider />

                    <Table
                        columnExtensions={tableColumnExtensions}
                        cellComponent={Cell}
                    />
                    <TableColumnReordering
                        order={columnOrder}
                        onOrderChange={this.changeColumnOrder}
                    />
                    <TableHeaderRow showSortingControls />
                    <TableEditRow
                        cellComponent={EditCell}
                    />
                    <TableEditColumn
                        width={170}
                        showAddCommand={!addedRows.length}
                        showEditCommand
                        showDeleteCommand
                        commandComponent={Command}
                    />
                    <TableFixedColumns
                        leftColumns={leftFixedColumns}
                    />
                    <PagingPanel
                        pageSizes={pageSizes}
                    />
                </Grid>
            </Paper>
        );
    }
}

const tempcontrol = withStyles(styles, { name: 'ControlledModeDemo' })(DemoBase);

export default connectTo(
    state => ({
        stateRows: state.manage.userList,
    }),
    { updateUser, goToErrorPage, deleteUser },
    tempcontrol
);
