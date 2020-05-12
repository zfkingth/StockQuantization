import React from 'react';
import Button from 'devextreme-react/button';
import DataGrid, { Column, Editing, Paging } from 'devextreme-react/data-grid';


import { updateUser, deleteUser } from '../../actions/auth';
import { goToErrorPage, } from '../../actions/navigation'

import { connectTo } from '../../utils/generic';




class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = { events: [] };
    this.logEvent = this.logEvent.bind(this);
    this.onEditingStart = this.logEvent.bind(this, 'EditingStart');
    this.onInitNewRow = this.logEvent.bind(this, 'InitNewRow');
    this.onRowInserting = this.logEvent.bind(this, 'RowInserting');
    this.onRowInserted = this.logEvent.bind(this, 'RowInserted');
    this.onRowUpdating = this.logEvent.bind(this, 'RowUpdating');
    this.onRowUpdated = this.logEvent.bind(this, 'RowUpdated');
    this.onRowRemoving = this.logEvent.bind(this, 'RowRemoving');
    this.onRowRemoved = this.logEvent.bind(this, 'RowRemoved');

    this.clearEvents = this.clearEvents.bind(this);
  }

  logEvent(eventName) {
    this.setState((state) => {
      return { events: [eventName].concat(state.events) };
    });
  }

  clearEvents() {
    this.setState({ events: [] });
  }


  updateRow = (e) => {

    const values = e.data;
    this.props.updateUser({
      id: values.id,
      values,
      reject: (message) => { this.props.goToErrorPage(message) }
    });
    this.logEvent('RowUpdated');

  }

  removeRow = e => {

    const row = e.data;
    this.props.deleteUser({
      id: row.id,
      reject: (message) => { this.props.goToErrorPage(message) }
    });

  }

  render() {
    const { rows, } = this.props;
    return (
      <React.Fragment>
        <DataGrid
          id="gridContainer"
          dataSource={rows}
          keyExpr="id"
          allowColumnReordering={true}
          showBorders={true}
          columnAutoWidth={true}
          width='100%'
          onEditingStart={this.onEditingStart}
          onInitNewRow={this.onInitNewRow}
          onRowInserting={this.onRowInserting}
          onRowInserted={this.onRowInserted}
          onRowUpdating={this.onRowUpdating}
          onRowUpdated={this.updateRow}
          onRowRemoving={this.onRowRemoving}
          onRowRemoved={this.removeRow}>

          <Paging enabled={true} />
          <Editing width={110}
            mode="row"
            allowUpdating={true}
            allowDeleting={true}
            allowAdding={true} />

          <Column width={120} dataField="email" caption="Email" />
          <Column width={120}  dataField="username" caption="Username" />
          <Column width={120}  dataField="roleName" caption="RoleName" />
          <Column width={120}  dataField="expiredDate" dataType="date" format="yyyy-MM-dd" />
        </DataGrid>

        <div id="events">
          <div>

            <div className="caption">Fired events</div>
            <Button id="clear" text="Clear" onClick={this.clearEvents} />
          </div>
          <ul>
            {this.state.events.map((event, index) => <li key={index}>{event}</li>)}
          </ul>
        </div>
      </React.Fragment>
    );
  }
}


export default connectTo(
  state => ({
    rows: state.manage.userList,
  }),
  { updateUser, goToErrorPage, deleteUser },
  App
);
