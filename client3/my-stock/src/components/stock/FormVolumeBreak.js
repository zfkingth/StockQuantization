import React from 'react';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Switch from '@material-ui/core/Switch';

import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';
import { connectTo } from '../../utils/generic';
import { searchVolumeBreak as searchAction, changeBaseDate } from '../../actions/stock';
import { submitAsyncValidation, transFormValuestoPostValues, isConnected } from '../../utils/forms'
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
        fontSize: 18
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
    volTimesNum: 2,
    zhangFu: 3,
    circleDaysNum: 10,
    nearDaysNum: 1,
    searchFromAllStocks: true,
};

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
                    搜索股票
                 </Button>
                <FormLabel disabled={!this.state.formHasError}
                    className={classes.menu}
                    error={this.state.formHasError}>
                    {this.state.formErrorMessage}
                </FormLabel>

                <FormControlLabel
                    control={
                        <Switch
                            className={classes.switch}
                            id='searchFromAllStocks'
                            checked={this.state.searchFromAllStocks}
                            onChange={this.switchChange('searchFromAllStocks')}

                        />
                    }
                    label="从所有股票列表中筛选"
                />



                <label className={classes.textField} >根据成交量进行筛选：</label>


                <TextField required
                    label="最近__日内成交量放大"
                    value={this.state.nearDaysNum}
                    onChange={this.handleChange('nearDaysNum')}
                    type="number"
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
                    variant="outlined"
                    margin="normal"
                />

                <TextField required
                    label="单日成交量突破__天日均成交量"
                    value={this.state.circleDaysNum}
                    onChange={this.handleChange('circleDaysNum')}
                    type="number"
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
                    variant="outlined"
                    margin="normal"
                />


                <TextField required
                    label="的__倍。"
                    value={this.state.volTimesNum}
                    onChange={this.handleChange('volTimesNum')}
                    type="number"
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
                <TextField required
                    label="并且单日涨幅不低于__%。"
                    value={this.state.zhangFu}
                    onChange={this.handleChange('zhangFu')}
                    type="number"
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

                <TextField required
                    id='baseDate'
                    label="搜索基准日期之前的数据"
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




