function[r]= corelatn(x,y);

%CORLATN   caculates the corelation spectra between the spectra of and
%          the concentration of sample.
%
% Written by Yuan Hongfu,1997,8,9, Beijing.

%      a    the spectra matrix.
%      c    the concentration vector.
%      avy  the average concentration value.
%      avx  the average spectra value at wavelength being treated.
%      r    the corelation spectruem. 
 
    [i,j]=size(x);
    x=x';
% if i>j,x=x';end;
   [i,j]=size(y);
if i<j,y=y';end;
 
        [m,n]=size(x);


        k=1;
        avy=(sum(y(:,k)))/m;
        sy=sqrt((sum((y(:,k)-avy).*(y(:,k)-avy)))/(m-1));   
            for j=1:n
          avx=(sum(x(:,j)))/m+eps;
          sx=sqrt((sum((x(:,j)-avx).*(x(:,j)-avx)))/(m-1))+eps;
          covxy=(sum((x(:,j)-avx).*(y(:,k)-avy)))/(m-1);
          r(:,j)=covxy/(sx*sy);
       end