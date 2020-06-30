import React from 'react';

import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';
import { connectTo } from '../../utils/generic';
import { searchSTAArise as searchAction, changeBaseDate  } from '../../actions/stock';
import { submitAsyncValidation, isConnected ,transFormValuestoPostValuesWithDateList} from '../../utils/forms'
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


export const getNextMonthDateText = () => {
    const temp = new Date();
    temp.setMonth(temp.getMonth() + 1);
  
  
  
    return temp.Format("yyyy-MM-dd");
  }


class TextFields extends React.PureComponent {

    state = {
        ...defaultValues,
        formHasError: false,
        formErrorMessage: '',
        baseDate:getNextMonthDateText(),
    };

    handleChange = name => event => {
        this.setState({ [name]: parseFloat(event.target.value) });
    };
    switchChange = name => event => {
        this.setState({ [name]: event.target.checked });
    };

    handleSubmit = fun => event => {
        const postModel = transFormValuestoPostValuesWithDateList(this.state, defaultValues, this.props.stockList, this.state.baseDate);
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
                    value={this.state.baseDate}
                    onChange={this.handleChange('baseDate')}
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
                <FormLabel >
                    {this.props.message}
                </FormLabel>




            </form>
        );
    }
}

TextFields.propTypes = {
    classes: PropTypes.object.isRequired,
};

const staResult = (stockList) => {

    const cnt = stockList.reduce((total, currentValue) => { if (currentValue.zhangDieFu > 0) total++; return total; }, 0);

    var s = "上涨概率为：" + _.round(100.0 * cnt / stockList.length, 2) + "%.";

    return s;

}



export default connectTo(
    state => ({
        enabledSubmit: state.stock.isIdle && isConnected(state),
        stockList: state.stock.stockList,
        baseDate: state.stock.baseDate,
        message: staResult(state.stock.stockList),
    }),
    { searchAction, changeBaseDate },
    withStyles(styles)(TextFields)
);




