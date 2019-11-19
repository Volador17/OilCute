%��׼��̬�����任��Standard Normal Variate Transformation��SNV����
%X������Ⱦ���
%SX������������Ⱦ���
function SX=snv(X)
	[rx,cx]=size(X);
	stdx=std(X,0,1);
	mx=mean(X,1);
	SX=(X-mx(ones(rx,1),:))./stdx(ones(rx,1),:);
end