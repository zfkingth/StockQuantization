import React from 'react';
import DataGrid, { Column, Editing } from 'devextreme-react/data-grid';
import Button from 'devextreme-react/button';


import { connectTo } from '../../utils/generic';

class DemoBase extends React.PureComponent {

  constructor(props) {
    super(props);
    this.state = { events: [] };
  

  }

  logEvent=(eventName) =>{
    this.setState((state) => {
      return { events: [eventName].concat(state.events) };
    });
  }

  clearEvents=() =>{
    this.setState({ events: [] });
  }

  isChief = (position) => {
    return position && ['CEO', 'CMO'].indexOf(position.trim().toUpperCase()) >= 0;
  }
  allowDeleting = (e) => {
    return !this.isChief(e.row.data.Position);
  }
  onRowValidating = (e) => {
    var position = e.newData.Position;

    if (this.isChief(position)) {
      e.errorText = `The company can have only one ${position.toUpperCase()}. Please choose another position.`;
      e.isValid = false;
    }
  }
  onEditorPreparing = (e) => {
    if (e.parentType === 'dataRow' && e.dataField === 'Position') {
      e.editorOptions.readOnly = this.isChief(e.value);
    }
  }
  isCloneIconVisible = (e) => {
    return !e.row.isEditing && !this.isChief(e.row.data.Position);
  }
  cloneIconClick = (e) => {
    // var employees = this.state.employees.slice(),
    //   clonedItem = Object.assign({}, e.row.data, { ID: service.getMaxID() });

    // employees.splice(e.row.rowIndex, 0, clonedItem);
    // this.setState({ employees: employees });
    const eventName=e.row.data.eventName;
    this.logEvent(eventName);
    e.event.preventDefault();
  }
  render() {

    const { rows, } = this.props;
    return (
      <React.Fragment>
      <DataGrid
        id="gridContainer"
        dataSource={rows}
        keyExpr="eventName"
        showBorders={true}
        onRowValidating={this.onRowValidating}
        onEditorPreparing={this.onEditorPreparing}>
        <Editing
          mode="row"
          useIcons={true}
         />
        <Column type="buttons" width={110}
          buttons={[ {
            hint: 'Arise',
            icon: 'repeat',
         
            onClick: this.cloneIconClick
          }]} />
        <Column dataField="eventName" caption="Title" />
        <Column dataField="lastAriseStartDate" dataType="date" />
        <Column dataField="lastAriseEndDate" dataType="date" />
        <Column dataField="status" />

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
    rows: state.manage.systemStatus.statusTable,
  }),
  {},
  DemoBase
);