import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';

import { connectTo } from '../../utils/generic';

const styles = theme => ({
  
    textField: {
        marginLeft: theme.spacing.unit,
        marginRight: theme.spacing.unit,
        width: 220,
    },
   
});
class TaskStatusLabel extends React.PureComponent {

    render() {
        const { classes, taskInfo } = this.props;
        return (

            <div>
                {taskInfo && 
                  <label className={classes.textField} >{taskInfo} </label>
                }
            </div>

        );
    }
}

TaskStatusLabel.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default connectTo(
    state => ({
        taskInfo: state.stock.taskInfo,
    }),
    {},
    withStyles(styles)(TaskStatusLabel)
);
