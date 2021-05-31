import React, { Component } from 'react';
import { TouchableOpacity, Text, TextInput, StyleSheet, View } from 'react-native';
import { saveSignUp } from './SignUpStorage.js';

export const usernameCheck = (info) => {

  if(info.username == '')
  {
    return "Must enter username";
  }
  else
  {
    return '';
  }
}
export const passwordCheck = (info) => {
    if (info.password == '')
    {
      return "Must enter password";
    }
    else if(info.password.length < 8)
    {
      return "Password must be atleast 8 characters";
    }
    else if (info.password.search(/[a-z]/) < 0)
    {
      return "Password must have atleast 1 lower case letter"
    }
    else if (info.password.search(/[A-Z]/) < 0)
    {
      return "Password must have atleast 1 upper case letter"
    }
    else if (info.password.search(/[0-9]/) < 0)
    {
      return "Password must have atleast 1 number"
    }
    else
    {
      return '';
    }
}

export const confirmPasswordCheck = (info) => {
  if(info.confirmPassword == '')
  {
    return "Must enter confirm password"
  }
  else if(info.confirmPassword != info.password)
  {
    return "Password must match";
  }
  else
  {
    return '';
  }
}

export const firstNameCheck = (info) => {

  if(info.firstName == '')
  {
    return "Must enter first name";
  }
  else
  {
    return '';
  }
}
export const lastNameCheck = (info) => {

  if(info.lastName == '')
  {
    return "Must enter last name";
  }
  else
  {
    return '';
  }
}
export const emailAddressCheck = (info) => {

  if(info.emailAddress == '')
  {
    return "Must enter email address";
  }
  else
  {
    return '';
  }
}
export const genderCheck = (info) => {

  if(info.gender == '')
  {
    return "Must enter gender";
  }
  else
  {
    return '';
  }
}
export const nationalityCheck = (info) => {

  if(info.nationality == '')
  {
    return "Must enter nationality";
  }
  else
  {
    return '';
  }
}
export const dateOfBirthCheck = (info) => {

  if(info.dateOfBirth == '')
  {
    return "Must enter date of birth";
  }
  else
  {
    return '';
  }
}
export const inputLineCheck = (info) => {

  if(info == '')
  {
    return "red";
  }
  else
  {
    return "#aaaaaa";
  }
}
export const passwordLineCheck = (info) => {

  if(info == '' || info < 8 || info.search(/[a-z]/) < 0 || info.search(/[A-Z]/) < 0 || info.search(/[0-9]/) < 0)
  {
    return "red";
  }
}
export const confirmPasswordLineCheck = (info) => {
  if(info.confirmPassword == '' || (info.confirmPassword != info.password))
  {
    return "red"
  }
  else
  {
    return "#aaaaaa";
  }
}
export const loginCheck = (info) => {

  if(info.username == '' || info.password.length < 8 || info.password.search(/[a-z]/) < 0 || info.password.search(/[A-Z]/) < 0 || info.password.search(/[0-9]/) < 0)
  {
    return "Username or Password is incorrect";
  }
  else
  {
    return '';
  }
}
