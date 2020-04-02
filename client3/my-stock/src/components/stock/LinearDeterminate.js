import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import LinearProgress from '@material-ui/core/LinearProgress';

import { connectTo } from '../../utils/generic';
const styles = {
    root: {
        flexGrow: 1,
        width: 200,
        margin: 20,

    },
};

class LinearDeterminate extends React.PureComponent {

    render() {
        const { classes, completed, } = this.props;
        return (

            <div className={classes.root} >
                {completed !== 0 && <LinearProgress variant="determinate" value={completed} />}
            </div>

        );
    }
}

LinearDeterminate.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default connectTo(
    state => ({
        completed: state.stock.completed,
    }),
    {},
    withStyles(styles)(LinearDeterminate)
);
