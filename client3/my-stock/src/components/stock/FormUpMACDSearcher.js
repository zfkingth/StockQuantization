import React from 'react';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Switch from '@material-ui/core/Switch';

import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';
import { connectTo } from '../../utils/generic';

//换一个action，这里的要相应变化 
import { searchMACD as searchAction, changeBaseDate } from '../../actions/stock';
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
    lowDiff: -0.02,
    lowDea: -0.04,
    preLowDiff: 0,
    preLowDea: 0,
    avgDays: 60,
    upDaysNum: 1,
    nearDaysNum: 30,
    exceptionNum: 2,
    searchFromAllStocks: false,
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



                <label className={classes.textField} >MACD条件筛选：</label>

                <TextField required
                    id='t1'
                    label="最近__日内MACD满足"
                    value={this.state.upDaysNum}
                    onChange={this.handleChange('upDaysNum')}
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
                    id='t21'
                    label="MACD的diff >= __"
                    value={this.state.lowDiff}
                    onChange={this.handleChange('lowDiff')}
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
                    id='t22'
                    label="MACD的dea >= __"
                    value={this.state.lowDea}
                    onChange={this.handleChange('lowDea')}
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
                    id='t3'
                    label="在之前__日内"
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
                    id='t3'
                    label="允许__次满足以下条件"
                    value={this.state.exceptionNum}
                    onChange={this.handleChange('exceptionNum')}
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
                    id='t31'
                    label="MACD的diff >= __"
                    value={this.state.preLowDiff}
                    onChange={this.handleChange('preLowDiff')}
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
                    id='t32'
                    label="MACD的dea >= __"
                    value={this.state.preLowDea}
                    onChange={this.handleChange('preLowDea')}
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




