import React from 'react';

import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';
import { connectTo } from '../../utils/generic';
import { searchSTAArise as searchAction, changeBaseDate } from '../../actions/stock';
import { submitAsyncValidation, isConnected } from '../../utils/forms'
import _ from 'lodash'
import { FormLabel } from '@material-ui/core';

const styles = theme => ({
    container: {
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'flex-start',
        flexWrap: 'wrap',


    },
    textField: {
        marginLeft: theme.spacing(1),
        marginRight: theme.spacing(1),
        width: 220,
    },
    switch: {

        marginLeft: theme.spacing(1),
    },

    formTextInput: {
        fontSize: 16
    },
    formTextLabel: {
        fontSize: 18
    },

    dense: {
        marginTop: 19,
    },
    menu: {
        width: 220,
    },
    button: {
        margin: theme.spacing(1),
    },
});

const defaultValues = {

    searchFromAllStocks: false,

};


function transFormValuestoPostValues(formValues, defaultValues, stockList, baseDate) {
    let ret = {};
    ret.baseDate = baseDate;
    for (let key in defaultValues) {
        ret[key] = formValues[key];
    }
    ret.StockIdList = stockList.map(function (item, index, array) {
        return item.stockId;
    });
   ret.dateList = stockList.map(function (item, index, array) {
        return item.date;
    });

    return ret;
}

class TextFields extends React.PureComponent {

    state = {
        ...defaultValues,
        formHasError: false,
        formErrorMessage: '',
    };

    handleChange = name => event => {
        this.setState({ [name]: parseFloat(event.target.value) });
    };
    switchChange = name => event => {
        this.setState({ [name]: event.target.checked });
    };

    handleSubmit = fun => event => {
        const postModel = transFormValuestoPostValues(this.state, defaultValues, this.props.stockList, this.props.baseDate);
        const promise = fun(postModel);
        promise.then(data => {
            //请求成功
            this.setState({
                formHasError: false,
                formErrorMessage: '',
            });
        },
            //请求失败 reject的回调
            message => {

                this.setState({
                    formHasError: true,
                    formErrorMessage: _.toString(message),
                });

            }

        )
    };

    render() {
        const { classes } = this.props;

        return (
            <form className={classes.container} noValidate autoComplete="off"
                onSubmit={
                    submitAsyncValidation(
                        this.handleSubmit,
                        this.props.enabledSubmit,
                        this.props.searchAction,
                    )}
            >


                <Button type="submit" disabled={!this.props.enabledSubmit} variant="contained" color="primary" className={classes.button}>
                    统计最高涨幅
                 </Button>
                <FormLabel disabled={!this.state.formHasError}
                    className={classes.menu}
                    error={this.state.formHasError}>
                    {this.state.formErrorMessage}
                </FormLabel>





                <TextField required
                    id='baseDate'
                    label="数据截止时间"
                    value={this.props.baseDate}
                    onChange={(e) => this.props.changeBaseDate(e.target.value)}
                    type="date"
                    className={classes.textField}
                    InputProps={{
                        classes: {
                            input: classes.formTextInput
                        }
                    }}
                    InputLabelProps={{
                        classes: {
                            root: classes.formTextLabel
                        }
                    }}
                    margin="normal"
                    variant="outlined"
                />


            </form>
        );
    }
}

TextFields.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default connectTo(
    state => ({
        enabledSubmit: state.stock.isIdle && isConnected(state),
        stockList: state.stock.stockList,
        baseDate: state.stock.baseDate,
    }),
    { searchAction, changeBaseDate },
    withStyles(styles)(TextFields)
);




